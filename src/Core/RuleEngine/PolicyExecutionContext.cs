using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace XecMe.Core.RuleEngine
{
    public class PolicyExecutionContext<T>: IDictionary<string, object>
    {
        private int _currentIndex;

        private T _this;

        private IList<T> _thisList;
        
        private Dictionary<string, object> _container = new Dictionary<string,object>();

        public PolicyExecutionContext(T thisObject)
        {
            _this = thisObject;
        }

        public PolicyExecutionContext(List<T> thisList)
        {
            _thisList = thisList;
        }

        public T This
        {
            get { return _this; }
            set { _this = value; }
        }

        public IList<T> ThisList
        {
            get { return _thisList; }
            set { _thisList = value; }
        }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; }
        }
        
        #region IDictionary<string,object> Members
        public void Add(string key, object value)
        {
            _container.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _container.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _container.Keys; }
        }

        public bool Remove(string key)
        {
            return _container.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            throw new NotImplementedException();
        }

        public ICollection<object> Values
        {
            get { return _container.Values; }
        }

        public object this[string key]
        {
            get
            {
                return _container[key];
            }
            set
            {
                this[key] = value;
            }
        }


        #endregion

        #region ICollection<KeyValuePair<string,object>> Members

        public void Add(KeyValuePair<string, object> item)
        {
            ((ICollection<KeyValuePair<string, object>>)_container).Add(item);
        }

        public void Clear()
        {
            _container.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_container).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)_container).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _container.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<string, object>>)_container).IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_container).Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,object>> Members

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<string, object>>)_container).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_container).GetEnumerator();
        }

        #endregion
    }
}
