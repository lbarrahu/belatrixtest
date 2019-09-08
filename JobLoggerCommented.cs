using System; 
using System.Linq; 
using System.Text;

public class JobLogger
{

    //the underscore notation as prefix names are correct since they work as private variables OK
    //We had better remove the keyword static to avoid problems if the class is invoqued more than once in a process BUT we need them static to save the rules on the first time its instanced, I think.

    private static bool _logToFile; //save in file
    private static bool _logToConsole; //print in console
    private static bool LogToDatabase; //save in DB, we should change the naming notation for this variable like the ones in previous lines in camel case and with an underscore as prefix

    //usar estas tres boleanas para ver que es de lo que se grabara el los si solo _logMessage o _logWarning o _logError
    private static bool _logMessage; //to choose what gets logged
    private static bool _logWarning; //to choose what gets logged
    private static bool _logError; //to choose what gets logged

    //never used.
    private bool _initialized;
    
    public JobLogger(bool logToFile, bool logToConsole, bool logToDatabase, bool logMessage, bool logWarning, bool logError)
    {
        _logError = logError;
        _logMessage = logMessage;
        _logWarning = logWarning;
        LogToDatabase = logToDatabase;
        _logToFile = logToFile;
        _logToConsole = logToConsole;
        _initialized = true;
    }
    

    public static void LogMessage(string message, bool message, bool warning, bool error) //here two parameters share the same name, it's incorrect CHANGE
    {
        message.Trim();
        if (message == null || message.Length == 0) // if the string is empty or null so it terminates execution of the method. WE CAN ADD AN EXCEPTION
        {
            return;
        }

        if (!_logToConsole && !_logToFile && !LogToDatabase)
        {
            //throwing a new error, because nothinG was selected: print, db or textfile. IT'S NECESARY TO END THE PROCESS
            throw new Exception("Invalid configuration"); 
        }
        if ((!_logError && !_logMessage && !_logWarning) || (!message && !warning && !error))
        { 
            throw new Exception("Error or Warning or Message must be specified");
            //IT'S NECESARY TO END THE PROCESS
        }

        System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]);
        connection.Open(); 


        int t;
        //if the message type is the same as the permitted, so it Logs.
        if (message && _logMessage)
        {
            t = 1;
        }
        if (error && _logError)
        {
            t = 2;
        }
        if (warning && _logWarning)
        {
            t = 3;
        }
        //IMPLEMENT TRY CATCH ()
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("Insert into Log Values('" + message + "', " + t.ToString() + ")");
        
        command.ExecuteNonQuery();
        //WARNING, NEED TO CLOSE CONNECTION.

        
        string l;

        //If it does not exist then read it, so it's a condition error, We had better implement a method that if the file doesn't exist, it creates that.
        if (!System.IO.File.Exists(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt"))
        {
            l = System.IO.File.ReadAllText(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt");
        }
        //if the message type is the same as the permitted, so it Logs. BUT IT DOESN'T INDICATE WHICH TYPE OF MESSAGE IS BEING WRITTEN.
        if (error && _logError)
        {
            l = l + DateTime.Now.ToShortDateString() + message;
        }
        if (warning && _logWarning)
        {
            l = l + DateTime.Now.ToShortDateString() + message;
        }
        if (message && _logMessage)
        {
            l = l + DateTime.Now.ToShortDateString() + message;
        }

        //WE SHOULD ADD A LINE, AND DO NOT HAVE TO OVERWRITE.
        System.IO.File.WriteAllText(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt", l);

        //if the message type is the same as the permitted, so it Logs.
        if (error && _logError)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        if (warning && _logWarning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        if (message && _logMessage)
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.WriteLine(DateTime.Now.ToShortDateString() + message);
    }
    
} 
  