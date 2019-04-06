#if !NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace XecMe.Common.System
{
    /// <summary>
    /// Controls the Cpu usage of the current .NET process around the limit value set.
    /// </summary>
    /// <remarks>
    /// By means of Cpu/Cores/ allocation and controlling the scheduling priority of process
    /// </remarks>
    public static class CpuUsageLimiter
    {
        private static bool _isStarted;
        /// <summary>
        /// Sampling interval for the CPU usage calculation
        /// </summary>
        private static int _samplingInterval = 500;
        /// <summary>
        /// Limit of the Cpu usage for this process
        /// </summary>
        private static float _limit;
        /// <summary>
        /// Current process in execution
        /// </summary>
        private static Process _currentProcess;
        /// <summary>
        /// This is the current Cpu mask, this will vary based on the Cpu usages
        /// </summary>
        private static long _currentCpuMask;
        /// <summary>
        /// Hold the priority sequence
        /// </summary>
        private static ProcessPriorityClass[] _processPriorities;
        /// <summary>
        /// Hold the current priority index
        /// </summary>
        private static int _priorityIndex;
        /// <summary>
        /// Upper and the lower limit for the process
        /// </summary>
        private static float _lowerLimit, _upperLimit;
        /// <summary>
        /// Cache the processor count to avoid calling the System Info call.
        /// </summary>
        private static readonly int ProcessorCount;

        /// <summary>
        /// Initializes the <see cref="CpuUsageLimiter"/> class.
        /// </summary>
        static CpuUsageLimiter()
        {
            //Initialize the priority class in the acending order
            _processPriorities = new ProcessPriorityClass[5];
            _processPriorities[0] = ProcessPriorityClass.Idle;
            _processPriorities[1] = ProcessPriorityClass.BelowNormal;
            _processPriorities[2] = ProcessPriorityClass.Normal;
            _processPriorities[3] = ProcessPriorityClass.AboveNormal;
            _processPriorities[4] = ProcessPriorityClass.High;
            //_processPriorities[5] = ProcessPriorityClass.RealTime;
            //Initialize to Normal
            _priorityIndex = 2;
            ProcessorCount = Environment.ProcessorCount;
            _currentProcess = Process.GetCurrentProcess();
            Limit = 10;
        }

        /// <summary>
        /// Gets the current win32 thread identifier.
        /// </summary>
        /// <returns></returns>
        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        private static extern Int32 GetCurrentWin32ThreadId();

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cpu Usage Limit can only be run in the default AppDomain</exception>
        public static void Start()
        {
            if (!AppDomain.CurrentDomain.IsDefaultAppDomain())
                throw new InvalidOperationException("Cpu Usage Limit can only be run in the default AppDomain");
            if (!_isStarted)
            {
                _isStarted = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(ControlUsage));
            }
        }


        /// <summary>
        /// Stops the current limiting and set the process to normal level
        /// </summary>
        public static void Stop()
        {
            _isStarted = false;
        }

        /// <summary>
        /// This method changes the priority of the current process
        /// </summary>
        /// <param name="increase">true will increase the process priority and false will reduce it</param>
        /// <returns>Returns true if priority been changed else false</returns>
        private static bool ChangePriority(bool increase)
        {
            if (increase)
            {
                if (_priorityIndex < _processPriorities.Length - 1)
                {
                    _priorityIndex++;
                    _currentProcess.PriorityClass = _processPriorities[_priorityIndex];
                    return true;
                }
            }
            else
            {
                if (_priorityIndex > 0)
                {
                    _priorityIndex--;
                    _currentProcess.PriorityClass = _processPriorities[_priorityIndex];
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Returns the priority level based on the priority level passed and the increase indicator
        /// </summary>
        /// <param name="current">Reference Priority level of that needs to be increased or decreased</param>
        /// <param name="increase">Indicator to increase or decrease the priority level</param>
        /// <returns>Changed priority level</returns>
        private static ThreadPriorityLevel GetPriority(ThreadPriorityLevel current, bool increase)
        {
            if (increase)
            {
                switch (current)
                {
                    case ThreadPriorityLevel.AboveNormal:
                        return ThreadPriorityLevel.Highest;
                    case ThreadPriorityLevel.BelowNormal:
                        return ThreadPriorityLevel.Normal;
                    case ThreadPriorityLevel.Idle:
                        return ThreadPriorityLevel.Lowest;
                    case ThreadPriorityLevel.Lowest:
                        return ThreadPriorityLevel.BelowNormal;
                    case ThreadPriorityLevel.Normal:
                        return ThreadPriorityLevel.AboveNormal;
                }
            }
            else
            {
                switch (current)
                {
                    case ThreadPriorityLevel.AboveNormal:
                        return ThreadPriorityLevel.Normal;
                    case ThreadPriorityLevel.BelowNormal:
                        return ThreadPriorityLevel.Lowest;
                    case ThreadPriorityLevel.Highest:
                        return ThreadPriorityLevel.AboveNormal;
                    case ThreadPriorityLevel.Lowest:
                        return ThreadPriorityLevel.Idle;
                    case ThreadPriorityLevel.Normal:
                        return ThreadPriorityLevel.BelowNormal;
                }
            }
            return current;
        }

        /// <summary>
        /// Increases or decreases the priority of a ProcessThread of the current process
        /// </summary>
        /// <param name="increase">Indicator to increase or decrease the priority level</param>
        /// <remarks>It is not advisable to call this method</remarks>
        private static void ChangeThreadPriority(bool increase)
        {
            ProcessThreadCollection threads = _currentProcess.Threads;
            int currentThreadId = GetCurrentWin32ThreadId();
            ProcessThread thread;
            for (int i = 0; i < threads.Count; i++)
            {
                thread = threads[i];
                if (thread.Id != currentThreadId)
                    thread.PriorityLevel = GetPriority(thread.PriorityLevel, increase);
            }
        }


        /// <summary>
        /// This method is called on a background thread to sample the Cpu usage and try to control it
        /// </summary>
        /// <param name="state">Dummy object used by the WaitCallback delegate</param>
        private static void ControlUsage(object state)
        {
            ///Save the original affinity
            ProcessPriorityClass originalPriority = _currentProcess.PriorityClass;
            IntPtr originalAffinity = _currentProcess.ProcessorAffinity;

            ProcessThread thisW32Thread = null;
            int threadID = GetCurrentWin32ThreadId();
            int allCpuMask = Convert.ToInt32(Math.Pow(2, ProcessorCount) - 1);
            for (int i = 0; i < _currentProcess.Threads.Count; i++)
            {
                ProcessThread pt = _currentProcess.Threads[i];
                if (pt.Id == threadID)
                {
                    thisW32Thread = pt;
                    pt.IdealProcessor = (int)allCpuMask;
                    pt.PriorityLevel = ThreadPriorityLevel.Highest;
                    break;
                }
            }

            using (PerformanceCounter cpuUsageCounter = new PerformanceCounter("Process", "% Processor Time", string.Format("{0}_{1}", _currentProcess.ProcessName, _currentProcess.Id)))
            using (CpuLoad cpuLoad = new CpuLoad())
            {
                while (_isStarted)
                {
                    ///Sample the usage
                    float cpuUsage = cpuUsageCounter.NextValue() / ProcessorCount;
                    //Usage is higher, let's reduce
                    if (cpuUsage > _upperLimit)
                    {
                        //It is higher than 10% 
                        if (cpuUsage > _upperLimit + 10)
                        {
                            //let's reduce the Cpu/Core/Thread count if possible
                            if (!cpuLoad.ReduceProcessor())
                            {
                                //Could not reduce the Cpu/Core/Thread for the process, let's change process priority
                                if (!ChangePriority(false))
                                {
                                    //Already a low priority process, let's put on the loaded Cpu/Core
                                    if (cpuLoad.Reduce())
                                    {
                                        _currentProcess.ProcessorAffinity = new IntPtr(cpuLoad.Mask);
                                    }
                                }
                            }
                            else
                            {
                                _currentProcess.ProcessorAffinity = new IntPtr(cpuLoad.Mask);
                            }
                            Console.WriteLine("< 10 No. of CPU {0}, Mask {1} Priority Process {2} Thread {3}", cpuLoad.ProcessorInUse, cpuLoad.Mask, _currentProcess.BasePriority, thisW32Thread.CurrentPriority);
                        }
                        //Cpu usage is not more than 10%, let's change process priority
                        else if (!ChangePriority(false))
                        {
                            //Already a low priority process, let's put on the loaded Cpu/Core
                            if (cpuLoad.Reduce())
                            {
                                _currentProcess.ProcessorAffinity = new IntPtr(cpuLoad.Mask);
                            }
                        }
                    }
                    else if (cpuUsage < _lowerLimit)
                    {
                        if (cpuUsage < _lowerLimit - 10)
                        {
                            if (!cpuLoad.IncreaseProcessor())
                            {
                                if (!ChangePriority(true))
                                {
                                    if (cpuLoad.Increase())
                                    {
                                        _currentProcess.ProcessorAffinity = new IntPtr(cpuLoad.Mask);
                                    }
                                }
                            }
                            else
                            {
                                _currentProcess.ProcessorAffinity = new IntPtr(cpuLoad.Mask);
                            }
                        }
                        else if (!ChangePriority(true))
                        {
                            if (cpuLoad.Increase())
                            {
                                _currentProcess.ProcessorAffinity = new IntPtr(cpuLoad.Mask);
                            }
                        }
                    }
                    Thread.Sleep(_samplingInterval);
                }
            }

            _currentProcess.ProcessorAffinity = originalAffinity;
            _currentProcess.PriorityClass = originalPriority;
        }

        /// <summary>
        /// Gets or sets the Cpu usage limit for the current process
        /// </summary>
        public static float Limit
        {
            get { return _limit; }
            set
            {
                if (value < 5 && value > 100)
                    throw new ArgumentOutOfRangeException();
                _limit = value;
                _lowerLimit = (float)(_limit - 4);
                _upperLimit = (float)(_limit);
            }
        }

        /// <summary>
        /// Gets or sets the sampling interval for the controlling
        /// </summary>
        public static int SamplingInterval
        {
            get { return _samplingInterval; }
            set
            {
                if (value < 500)
                    throw new ArgumentOutOfRangeException("SamplingInterval");
                _samplingInterval = value;
            }
        }

        #region Processor        
        /// <summary>
        /// Represents the core of a processor
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        private class Processor : IDisposable
        {
            /// <summary>
            /// Performance counter for a Cpu/Core/Thread
            /// </summary>
            private PerformanceCounter _cpu;
            /// <summary>
            /// Mask for the current processor
            /// </summary>
            private long _mask;
            /// <summary>
            /// Gets or sets whether to use this processor for the current process.
            /// </summary>
            private bool _inUse;
            /// <summary>
            /// Gets the snapshot of the usage.
            /// </summary>
            private float _usage;
            /// <summary>
            /// Creates the processor object
            /// </summary>
            /// <param name="processorNumber">processor no</param>
            public Processor(int processorNumber)
            {
                _cpu = new PerformanceCounter("Processor", "% Processor Time", processorNumber.ToString());
                _inUse = true;
                _mask = Convert.ToInt64(Math.Pow(2, processorNumber));
            }

            /// <summary>
            /// Gets the mask.
            /// </summary>
            /// <value>
            /// The mask.
            /// </value>
            public long Mask
            {
                get
                {
                    ThrowIfDisposed();
                    if (_inUse)
                        return _mask;
                    else
                        return 0;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether [in use].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [in use]; otherwise, <c>false</c>.
            /// </value>
            public bool InUse
            {
                get
                {
                    ThrowIfDisposed();
                    return _inUse;
                }
                set
                {
                    ThrowIfDisposed();
                    _inUse = value;
                }
            }

            /// <summary>
            /// Gets the usage.
            /// </summary>
            /// <value>
            /// The usage.
            /// </value>
            public float Usage
            {
                get
                {
                    ThrowIfDisposed();
                    return _usage;
                }
            }

            /// <summary>
            /// Samples the usage.
            /// </summary>
            public void SampleUsage()
            {
                ThrowIfDisposed();
                _usage = _cpu.NextValue();
            }

            #region IDisposable Members            
            /// <summary>
            /// Throws if disposed.
            /// </summary>
            /// <exception cref="ObjectDisposedException">This instance is already disposed</exception>
            private void ThrowIfDisposed()
            {
                if (_cpu == null)
                    throw new ObjectDisposedException("This instance is already disposed");
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            void IDisposable.Dispose()
            {
                using (_cpu) ;
                _cpu = null;
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="Processor"/> class.
            /// </summary>
            ~Processor()
            {
                IDisposable dispose = (IDisposable)this;
                dispose.Dispose();
            }

            #endregion
        }
        #endregion

        #region CpuLoad
        /// <summary>
        /// This class keeps track of the Cpus/Cores/Cpu Threads and its load.
        /// </summary>
        private class CpuLoad : IDisposable
        {
            /// <summary>
            /// List of the Cores/H/W Threads and its usages
            /// </summary>
            private List<Processor> _processors;
            /// <summary>
            /// List the number of processor used for the calculations
            /// </summary>
            private int _processorInUse;
            /// <summary>
            /// Comparer used for the comparing the load on the different cores
            /// </summary>
            private UsageComparer _comparer;


            /// <summary>
            /// Constructor for the this class.
            /// </summary>
            /// <remarks>Initializes the Processor class per Core/Thread</remarks>
            public CpuLoad()
            {
                int count = _processorInUse = ProcessorCount;
                _processors = new List<Processor>();
                _comparer = new UsageComparer();
                for (int i = 0; i < count; i++)
                {
                    _processors.Add(new Processor(i));
                }
            }

            /// <summary>
            /// Reduces the processor by 1 for calculating the load.
            /// </summary>
            /// <returns>Returns true in case of successful reducing the Cpu</returns>
            public bool ReduceProcessor()
            {
                ThrowIfDisposed();
                if (_processorInUse == 1)
                    return false;
                for (int i = 0; i < _processors.Count; i++)
                {
                    if (_processors[i].InUse)
                    {
                        _processors[i].InUse = false;
                        _processorInUse--;
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Increases the Cpu/Core by 1 to increase the processing capacity
            /// </summary>
            /// <returns>Returns true, in case of successful reductions in processing power</returns>
            public bool IncreaseProcessor()
            {
                ThrowIfDisposed();
                if (_processorInUse == ProcessorCount)
                    return false;
                for (int i = 0; i < _processors.Count; i++)
                {
                    if (!_processors[i].InUse)
                    {
                        _processors[i].InUse = true;
                        _processorInUse++;
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Reduces the processing power by moving on to more loaded Core/Cpu/Thread
            /// </summary>
            /// <returns>Returns true, if it can find a more loaded processor. </returns>
            /// <remarks>Also it reduces the processors in case if the process is already on loaded Cpu</remarks>
            public bool Reduce()
            {
                int o, n;
                if (_processors.Count == 1 || _processorInUse == _processors.Count)
                    return false;
                o = n = -1;
                LoadUsage();
                for (int i = _processors.Count - 1; i >= 0; i--)
                {
                    if (_processors[i].InUse && o == -1)
                        o = i;
                    if (o != -1 && !_processors[i].InUse)
                    {
                        n = i;
                        break;
                    }
                }

                if (o != -1)
                {
                    if (n != -1)
                    {
                        _processors[o].InUse = false;
                        _processors[n].InUse = true;
                        return true;
                    }
                    else
                    {
                        if (_processorInUse > 1)
                        {
                            _processorInUse--;
                            _processors[0].InUse = false;
                            return true;
                        }
                    }
                }
                return false;
            }

            /// <summary>
            /// Increases the processing power by moving on to more loaded Core/Cpu/Thread
            /// </summary>
            /// <returns>Returns true, if it can find a more loaded processor. </returns>
            /// <remarks>Also it increases the processors in case if the process is already on relatively free Cpu</remarks>
            public bool Increase()
            {
                int o, n;
                if (_processors.Count == 1 || _processorInUse == _processors.Count)
                    return false;
                o = n = -1;
                LoadUsage();
                for (int i = 0; i < _processors.Count; i++)
                {
                    if (_processors[i].InUse && o == -1)
                        o = i;
                    if (o != -1 && !_processors[i].InUse)
                    {
                        n = i;
                        break;
                    }
                }

                if (o != -1)
                {
                    if (n != -1)
                    {
                        _processors[o].InUse = false;
                        _processors[n].InUse = true;
                        return true;
                    }
                    else
                    {
                        if (_processorInUse < ProcessorCount)
                        {
                            _processorInUse++;
                            _processors[0].InUse = true;
                            return true;
                        }
                    }
                }
                return false;
            }

            /// <summary>
            /// Returns the number of processors in use
            /// </summary>
            public int ProcessorInUse
            {
                get
                {
                    ThrowIfDisposed();
                    return _processorInUse;
                }
            }

            /// <summary>
            /// Returns the processor mask based on the load of this instance
            /// </summary>
            public long Mask
            {
                get
                {
                    ThrowIfDisposed();
                    long mask = 0;
                    for (int i = 0; i < _processors.Count; i++)
                        mask += _processors[i].Mask;
                    return mask;
                }
            }

            /// <summary>
            /// Takes a snapshot of the load for all the Cpu/Core/Thread instances.
            /// </summary>
            public void LoadUsage()
            {
                ThrowIfDisposed();
                for (int i = 0; i < _processors.Count; i++)
                {
                    _processors[i].SampleUsage();
                }
                _processors.Sort(0, _processors.Count, _comparer);
            }


            #region IDisposable Members            
            /// <summary>
            /// Throws if disposed.
            /// </summary>
            /// <exception cref="ObjectDisposedException">This instance is already dispossed</exception>
            private void ThrowIfDisposed()
            {
                if (_processors == null)
                    throw new ObjectDisposedException("This instance is already dispossed");
            }
            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            void IDisposable.Dispose()
            {
                for (int i = 0; i < _processors.Count; i++)
                    using (_processors[i]) ;
                _processors = null;
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="CpuLoad"/> class.
            /// </summary>
            ~CpuLoad()
            {
                IDisposable disposable = (IDisposable)this;
                disposable.Dispose();
            }
            #endregion

            /// <summary>
            /// Processor/core comparer
            /// </summary>
            /// <seealso cref="System.Collections.Generic.IComparer{XecMe.Common.System.CpuUsageLimiter.Processor}" />
            private class UsageComparer : IComparer<Processor>
            {
                #region IComparer<Processor> Members                
                /// <summary>
                /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
                /// </summary>
                /// <param name="x">The first object to compare.</param>
                /// <param name="y">The second object to compare.</param>
                /// <returns>
                /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
                /// </returns>
                int IComparer<Processor>.Compare(Processor x, Processor y)
                {
                    if (x == null && y == null)
                        return 0;
                    if (x == null && y != null)
                        return -1;
                    if (x != null && y == null)
                        return 1;
                    float xVal, yVal;
                    xVal = x.Usage;
                    yVal = y.Usage;
                    if (xVal == yVal)
                        return 0;
                    if (xVal < yVal)
                        return -1;
                    else
                        return 1;
                }

                #endregion
            }

        }
        #endregion

    }

}
#endif