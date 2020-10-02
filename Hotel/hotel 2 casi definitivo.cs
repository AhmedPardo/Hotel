using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Channels;

namespace Hotel
{
    class Program
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["Hotel"].ConnectionString;
        static SqlConnection conexion = new SqlConnection(connectionString);
        static string cadena;
        static SqlCommand comando;
        static SqlDataReader registros;


        static void Main(string[] args)
        {
            Console.WriteLine("Introduce 1 si quieres Registrar Cliente \n " +
                               "Introduce 2 si quieres Editar cliente\n" +
                               "Introduce 3 si quieres hacer el Cheack\n" +
                              "Indtroduce 4 si quieres hacer el Cheack out\n" +
                              "Introduce 5 para ver el listado de todas las habitaciones\n" +
                              "Introduce 6 para ver listado de habitaciones ocupadas y sus huespedes\n" +
                              "Introduce 7 para ver el listado de las habitaciones vacias");
            string opcion1 = (Console.ReadLine());
            if (opcion1 == "1")
            {
                RegistrarCliente();
            }

            if (opcion1 == "2")
            {
                EditarCliente();

            }
            if (opcion1 == "3")
            {
                Cheack();

            }
            if (opcion1 == "4")
            {
                CheckOut();
            }
            if (opcion1 == "5")
            {
                VerHabitaciones();
            }
            if (opcion1 == "6")
            {
                HabitacionesOcupadas();
            }
            if (opcion1 == "7")
            { HabitacionesVacias(); }

        }
        static void RegistrarCliente()
        {
            Console.WriteLine("Inserte su nombre ");
            string nombre = Console.ReadLine();
            Console.WriteLine("Inserte su apellido");
            string apellidos = Console.ReadLine();
            Console.WriteLine("Inserte Dni, por favor");
            string Dni = Console.ReadLine();
            conexion.Open();
            cadena = $"INSERT INTO Tablaclientes VALUES ('{nombre}', '{apellidos}','{Dni}')";
            comando = new SqlCommand(cadena, conexion);
            comando.ExecuteNonQuery();
            conexion.Close();
        }

        static void EditarCliente()

        {
            string dnicorrecto = "";
            bool reg = false;
            do
            {
                Console.WriteLine("Introduce el Dni del cliente al que quieres cambiar los datos, por favor");
                dnicorrecto = Console.ReadLine();
                conexion.Open();
                cadena = $"SELECT * FROM Tablaclientes where Dni= ('{dnicorrecto}')";
                comando = new SqlCommand(cadena, conexion);
                registros = comando.ExecuteReader();
                if (registros.Read())
                {
                    reg = true;
                }
                conexion.Close();

            } while (reg = false);


            conexion.Open();
            Console.WriteLine("Introduce NUEVO nombre ");
            string respuesta = Console.ReadLine();
            Console.WriteLine("Introduce Nuevo apellido madafaka");
            string respuesta2 = Console.ReadLine();
            cadena = $"UPDATE Tablaclientes SET Nombre = '{respuesta}', Apellidos = '{respuesta2}' WHERE Dni = '{dnicorrecto}'; ";
            comando = new SqlCommand(cadena, conexion);
            comando.ExecuteNonQuery();
            conexion.Close();
        }
        static void Cheack()
        {



            Console.WriteLine("Introduce el Dni del cliente que quiere hacer la reserva");
            string Dni = Console.ReadLine();
            conexion.Open();
            cadena = $"SELECT * FROM Tablaclientes where Dni ='{Dni}'";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();

            if (registros.Read())
            {
                conexion.Close();
                conexion.Open();

                cadena = $"SELECT * FROM Habitaciones where Estado= 'True'";
                comando = new SqlCommand(cadena, conexion);
                registros = comando.ExecuteReader();
                while (registros.Read())
                {
                    Console.WriteLine(registros["NombreHabitacion"] + " Estado disponible ");
                }
            }
            else
            {
                Console.WriteLine("El cliente no esta registrado,por tanto no puede hacer una reserva");
            }
            conexion.Close();
            int habSeleccionada = ValidarHabitacion();
            conexion.Close();
            conexion.Open();

            Console.WriteLine("Introduce la fecha de entrada");

            DateTime fecha = Convert.ToDateTime(Console.ReadLine());
            cadena = $"Insert into  Reservas( Dni, FechaCheking, CodReserva, CodHabitacion) VALUES ('{Dni}','{fecha}',1,'{habSeleccionada}')";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();
            conexion.Close();

        }

        static int ValidarHabitacion()
        {
            Console.WriteLine("Introduce que habitacion quieres");
            int habitacion = int.Parse(Console.ReadLine());

            while (!(habitacion < 9 && habitacion > 0))
            {
                Console.WriteLine("Esta habitacion es la del panico, no disponible para huespedes");
                habitacion = int.Parse(Console.ReadLine());
            }
            Console.WriteLine("Has elegido" + habitacion);



            Update(habitacion);
            return habitacion;

        }
        static void Update(int habitacion)

        {
            cadena = $"UPDATE Habitaciones SET Estado = 'False' WHERE CodHabitaciones = { habitacion }";
            conexion.Open();
            comando = new SqlCommand(cadena, conexion);
            comando.ExecuteNonQuery();
        }

        static void CheckOut()
        {


            Console.WriteLine("Introduce un Dni");
            string DniCorrecto = Console.ReadLine();
            string habCode = "";
            conexion.Open();
            cadena = $"SELECT * FROM Reservas where Dni ='{ DniCorrecto }'";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();
            if (registros.Read())
            {
                habCode = registros[4].ToString();
            }
            conexion.Close();
            conexion.Open();
            Console.WriteLine("¿Cuando quieres abandonarnos?");
            DateTime fecha = Convert.ToDateTime(Console.ReadLine());
            cadena = $"UPDATE Reservas SET  FechaCheakOut = '{ fecha }' where Dni ='{ DniCorrecto }'";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();
            conexion.Close();
            conexion.Open();
            cadena = $"UPDATE Habitaciones SET Estado = 'False' WHERE CodHabitacion = { habCode }";
            registros = comando.ExecuteReader();
            //no me incluye la fecha en la base de datos//

            conexion.Close();
            Console.WriteLine("Vuelve pronto,te echaremos de menos :(");




        }

        static void VerHabitaciones()
        {

            conexion.Open();
            cadena = $"SELECT * FROM Habitaciones FULL OUTER JOIN Reservas ON Reservas.CodHabitacion = Habitaciones.CodHabitaciones FULL OUTER JOIN Tablaclientes ON Tablaclientes.Dni = Reservas.Dni";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();


            {
                while (registros.Read())
                {
                    Console.WriteLine(registros["Estado"].ToString() + "\t" + registros["NombreHabitacion"].ToString() + "\t" + registros["Nombre"].ToString() + "\t" + registros["Apellidos"].ToString());

                }
            }
            conexion.Close();

        }

        static void HabitacionesOcupadas()
        {

            conexion.Open();
            cadena = "SELECT * FROM Habitaciones FULL OUTER JOIN Reservas ON Reservas.CodHabitacion = Habitaciones.CodHabitaciones and Habitaciones.Estado = 0 FULL OUTER JOIN Tablaclientes ON Tablaclientes.Dni = Reservas.Dni where Habitaciones.Estado='false'";
            comando = new SqlCommand(cadena, conexion);
            registros = comando.ExecuteReader();


            Thread.Sleep(2000);
                while (registros.Read())
                {
                    Console.WriteLine(registros["Estado"].ToString() + "\t" + registros["Nombre"].ToString() + "\t" + registros["Apellidos"].ToString());

                } conexion.Close();
            }
   


        static void HabitacionesVacias()
        {
            
                conexion.Open();
                cadena = "SELECT * FROM Habitaciones FULL OUTER JOIN Reservas ON Reservas.CodHabitacion = Habitaciones.CodHabitaciones and Habitaciones.Estado = 1 FULL OUTER JOIN Tablaclientes ON Tablaclientes.Dni = Reservas.Dni where Habitaciones.Estado=1";
                comando = new SqlCommand(cadena, conexion);
                registros = comando.ExecuteReader();

                while (registros.Read())
                {
                    Console.WriteLine(registros["NombreHabitacion"].ToString() + "\t" + registros["Nombre"].ToString() + "\t" + registros["Apellidos"].ToString());
                }
             conexion.Close();

        
    }
}}










