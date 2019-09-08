# belatrixtest

## Code Review

Please review the following code snippet. Assume that all referenced assemblies have been properly included.
The code is used to log different messages throughout an application. We want the ability to be able to log to a text file, the console and/or the database. 
Messages can be marked as message, warning or error. 
We also want the ability to selectively be able to choose what gets logged, such as to be able to log only errors or only errors and warnings.

1) If you were to review the following code, what feedback would you give? Please be specific and indicate any errors that would occur as well as other best practices and code refactoring that should be done.

### As I see there are some errors:
```
Two parameters of a method have the same name.
Naming conventions. LogToDatabase is in PascalCase and the others in camelCase.
Open and close DB connection.
Some validations do not end the process.
Exceptions handling.
One method has different functions, so we had better separate it.

I saw too that a rule was missing, to control whether the Log was displayed on the console, or saved in the database or in a file.

I left the file JobLoggerCommented which has the code baseline, with comments on the lines that I think would have errors.
```
### Code refactoring:
```
Looking for a simple Log. I would make the following changes:
First I'm going to correct the errors mentioned above.
The same validations are repeated several times, only to define whether the type of message matched the allowed one, then proceed to  log to a text file, the console and/or the database.
So we had better separate the functions of displaying in console, writing to a text file and saving in the database.
I separated other functions too, as define the type of the message that will be Logged, and other to launch the Logs.
```

2) Rewrite the code based on the feedback you provided in question 1. Please include unit tests on your code.

See the  files JobLoggerCorrected and JobLoggerCorrected_Test.
