using System; 
using System.Linq; 
using System.Text; 
 
public class JobLogger 
{ 
 
    private static bool _logToFile; //grabar en archivo
    private static bool _logToConsole; //imprimir en consola
    private static bool _logMessage; //para pintar como mensaje
    private static bool _logWarning; //para pintar como alerta
    private static bool _logError; //para pintar como error
    private static bool LogToDatabase; //grabar en DB //podemos cambiar la notación de este nombre de propiedad. a camel case como las demás, ahora está en pascal
    private bool _initialized; 
    //constructor
    public JobLogger(bool logToFile, bool logToConsole, bool logToDatabase, bool logMessage, bool logWarning, bool logError) //constructor change the notation
    { 
        _logError = logError; 
        _logMessage = logMessage; 
        _logWarning = logWarning; 
        LogToDatabase = logToDatabase; 
        _logToFile = logToFile; 
        _logToConsole = logToConsole; 
    } 
 
    #region Methods to Log messages, alerts or errors

    public static void LogMessage(string message, bool message, bool warning, bool error) //here two parameters share the same name, it's incorrect
    { 
        message.Trim(); //corta los espacios adelante y atras
        if (message == null || message.Length == 0) 
        { 
            return; 
        } 
        if (!_logToConsole && !_logToFile && !LogToDatabase) 
        { 
            throw new Exception("Invalid configuration"); 
        } 
        if ((!_logError && !_logMessage && !_logWarning) || (!message && !warning && !error)) 
        { 
            throw new Exception("Error or Warning or Message must be specified"); 
        } 
 
        System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]); //ya esta declarada la conexión correctamente
        connection.Open(); 


        int t; 
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

        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("Insert into Log Values('" + message + "', " + t.ToString() + ")"); 
        command.ExecuteNonQuery(); 
 
        string l; 
        if (!System.IO.File.Exists(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt")) 
        { 
            l = System.IO.File.ReadAllText(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt"); 
        } 
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
        System.IO.File.WriteAllText(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt", l); 
 
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
    #endregion
} 
  