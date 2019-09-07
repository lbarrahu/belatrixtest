using System; 
using System.Linq; 
using System.Text;

public class JobLogger
{

    private static bool _logToFile; //save in file
    private static bool _logToConsole; //print in console
    private static bool _logToDatabase; //save in DB
    private static bool _logMessage; //to choose what gets logged
    private static bool _logWarning; //to choose what gets logged
    private static bool _logError; //to choose what gets logged

    public JobLogger(bool logToFile, bool logToConsole, bool logToDatabase, bool logMessage, bool logWarning, bool logError)
    {
        _logError = logError;
        _logMessage = logMessage;
        _logWarning = logWarning;
        _logToDatabase = logToDatabase;
        _logToFile = logToFile;
        _logToConsole = logToConsole;
    }
    
    public static void LogMessage(string messageText, bool message, bool warning, bool error)
    {
        messageText.Trim();
        if (messageText == null || messageText.Length == 0)
        {
            throw new Exception("Not message sent.");
            return;
        }
        if (!_logToConsole && !_logToFile && !LogToDatabase)
        {
            throw new Exception("Invalid configuration"); 
            return;
        }
        if (!message && !warning && !error)
        {
            throw new Exception("Error or Warning or Message must be specified as type of message");
            return;
        }
        if (!_logError && !_logMessage && !_logWarning)
        {
            throw new Exception("Type of Log permitted must be specified.");
            return;
        }
        int t = DefineMessageType(message, warning, error);

        if ((_logWarning && warning)||(_logError && error) || (_logMessage && message))
        {
            DoLogTypesPermitted(messageText, t);
        }

    }
    private void DoLogTypesPermitted(string messageText, int t)
    {
        if (_logToConsole)
        {
            LogInConsole(messageText, t);
        }
        if (_logToDatabase)
        {
            LogInDataBase(messageText, t);
        }
        if (_logToFile)
        {
            LogInTextFile(messageText, t);
        }
    }

    private int DefineMessageType(bool message, bool warning, bool error)
    {
        if (message)
        {
            return 1;
        }
        if (error)
        {
            return 2;
        }
        if (warning)
        {
            return 3;
        }
    }
    
    private void LogInTextFile(string message, int t)
    {
        string type = "";
        switch (t)
        {
            case 2:
                type = "Error";
                break;
            case 3:
                type = "Alert";
                break;
            default:
                type = "Message";
                break;
        }
        
        string path = System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt";
        if (!System.IO.File.Exists(path))
        {
            using (System.IO.StreamWriter sw = System.IO.File.CreateText(path))
            {
                sw.WriteLine(DateTime.Now.ToShortDateString() + " " + type + ": " + message);
            }
        }
        else
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true))
            {
                sw.WriteLine(DateTime.Now.ToShortDateString() + " " + type + ": " + message);
            }
        }

    }

    private void LogInConsole(string message, int t)
    {
        if (t == 2)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        if (t == 3)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        if (t == 1)
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.WriteLine(DateTime.Now.ToShortDateString() + message);
    }

    private void LogInDataBase(string message, int t)
    {
        System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]); //ya esta declarada la conexión correctamente
        
        string insertString = "Insert into Log Values('" + message + "', " + t.ToString() + ")";
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(insertString, connection);
        try
        {
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new Exception("Error inserting record in the database: " + ex.Message);
        }
        if (connection != null)
        {
            connection.Close();
        }
    }

} 
  