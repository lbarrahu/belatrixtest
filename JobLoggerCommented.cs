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
    private static bool _logMessage; //to paint as a message
    private static bool _logWarning; //to paint as an alert
    private static bool _logError; //to paint as an error

    private bool _initialized;

    //constructor HERE WE COULD ADD ONE ELSE CONSTRUCTOR METHOD WITH NO PARAMETER, BUT THERE ARE NO public properties but LogMessage().
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
        message.Trim(); //removes all leading and trailing white-space
        if (message == null || message.Length == 0) // if the string is empty or null so it terminates execution of the method. WE CAN ADD AN EXCEPTION
        {
            return;
        }

        if (!_logToConsole && !_logToFile && !LogToDatabase)
        {
            //throwing a new error, because nothinG was selected print, db or textfile. IT'S NECESARY TO END THE PROCESS
            throw new Exception("Invalid configuration"); 
        }
        if ((!_logError && !_logMessage && !_logWarning) || (!message && !warning && !error))
        { 
            //Correcto, el tipo de mensaje debe ser al menos uno delos tres tipos, y el tipo de log permitido debe ser al menos uno de los tres.
            throw new Exception("Error or Warning or Message must be specified");
            //IT'S NECESARY TO END THE PROCESS
        }

        System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings["ConnectionString"]); //CADENA DE CONEXIÓN
        //aquí ya abrió conexión
        connection.Open(); 


        int t;
        //valida que si el mensaje esta de un tipo que esta permitido hacerse LOG, entonces lo hace.
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
        //falto enviar la conexión ERROR IMPLEMENTAR TRY CATCH (PUEDE QUE T NUNCA OBTENGA SU VALOR)
        System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("Insert into Log Values('" + message + "', " + t.ToString() + ")");
        //es correcto porque no se espera valor de la ejecución
        command.ExecuteNonQuery(); 
        //es necesario cerrar la conexión WARNING

        //en esta cadena graba el texto que está en el archivo de logs
        string l;

        //si no existe el archivo entonces lo lee, ESTA MAL, mejor creamos un metodo que si no existe lo crea y escribe y si sí existe abre el archivo para agregarle una línea 
        if (!System.IO.File.Exists(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt"))
        {
            l = System.IO.File.ReadAllText(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt");
        }
        //valida que si el mensaje esta de un tipo que esta permitido hacerse LOG, entonces lo hace. pero no escribe que tipo de mensaje era. T
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

        //ahora sobreescribe el contenido del log mantener el contenido del log en una variable puede generar problemas si el contenido del los es muy grande.
        System.IO.File.WriteAllText(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt", l);

        //valida que si el mensaje esta de un tipo que esta permitido hacerse LOG, entonces lo hace.
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
  