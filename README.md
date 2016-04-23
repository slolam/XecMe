#XecMe
**XecMe** in short for Execute Me, is a executions and hosting application block for the batches, background processes and Windows Services. It is a highly configurable and extensible framework. It follows the principles of task oriented design approach for solving the business problem.

 Now the task on the designer and developer is to solve the problem in a task based approach. Let us take some examples of solving the problem.

**Batch job to generate a file and post it to a partner's FTP site.**
There are 3 main activities in this job. Retrieving data from the data source, creating a file and posting it to a FTP site. One can code all the 3 activities in to a single ITask implementation. However it is recommended to break this problem into 3 tasks viz. 
1.Retrieving from the database 
2.Generating the file 
3.Posting a file to FTP site

Why? Well, by breaking it into smaller sub tasks will enable us to reuse the tasks in other area. You have write a generic task that takes DataSet or List of Entities to generates a file based on some configuration (comma separated, tab separated etc). You can reuse this task in many other file generation process. In similar way, you can write a file uploading task that you can reuse it at all the place where you have similar need. 
 You can wire tasks one after another thru the configuration.

**Order processing from a queue**
You can write one task for reading the queue and other task to process and update the database. This way the queue reading task can be generic and can be used in multiple other solutions. Queue reading task can be wired to the processing task thru configuration.

**Azure Worker Role or WebJob**
Here is the sample to use XecMe in developing  Worker Role
