using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;

namespace Smart_Mart_Stock_Monitoring_System
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        ////////////////////////////////////////////////////////////////////////////////////// 1ST USER STORY //////////////////////////////////////////////////////////////
        #region
        private void Add(object sender, RoutedEventArgs e) //Navigate from the home menu to the add item menu
        {
            Home.Visibility = Visibility.Hidden;
            Add_Menu.Visibility = Visibility.Visible;
        }
        private void Submit(object sender, RoutedEventArgs e) //Commits item data to database
        {
            //Make connection to database
            using (System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection("data source= D:\\University\\Mini_Market.db"))
            {
                using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(conn))
                {

                    conn.Open(); //Open connection to database
                    //Write the query to add a new item
                    cmd.CommandText = "INSERT INTO Items(id,name,price,qty,arrivaldate,minreq,maxreq,staffcheck,orderplaced) values(@ID,@Name,@Price,@Quantity,@Arrival,@Min,@Max,@Check,@Order)";
                    //Get values from the text boxes
                    cmd.Parameters.AddWithValue("@ID", Item_ID.Text);
                    cmd.Parameters.AddWithValue("@Name", Item_Name.Text);
                    cmd.Parameters.AddWithValue("@Price", Item_Price.Text);
                    cmd.Parameters.AddWithValue("@Quantity", Item_Quantity.Text);
                    cmd.Parameters.AddWithValue("@Arrival", Item_Arrival.Text);
                    cmd.Parameters.AddWithValue("@Min", Min_req.Text);
                    cmd.Parameters.AddWithValue("@Max", Max_req.Text);
                    cmd.Parameters.AddWithValue("@Check", Staff_Check.Text);
                    cmd.Parameters.AddWithValue("@Order", "FALSE");
                    try
                    {
                        cmd.ExecuteNonQuery(); //Execute the query to commit changes
                        //Empty the the text boxes
                        Item_Name.Text = "";
                        Item_Price.Text = "";
                        Item_Quantity.Text = "";
                        Item_Arrival.Text = "DD/MM/YYYY";
                        Item_ID.Text = "";
                        Min_req.Text = "";
                        Max_req.Text = "";
                        Staff_Check.Text = "DD/MM/YYYY";
                        Added.Content = "Item added to database";
                    }
                    catch
                    {
                        Added.Content = "Wrong input details";
                    }
                    finally
                    {
                        conn.Close(); //Close the database
                    }
                }
            }
        }
        private void Add_Home(object sender, RoutedEventArgs e) //Navigate from the add item menu to the home menu
        {
            Add_Menu.Visibility = Visibility.Hidden;
            Home.Visibility = Visibility.Visible;
        }
#endregion

        ////////////////////////////////////////////////////////////////////////////////////// 2ND USER STORY /////////////////////////////////////////////////////////////
        #region
        private void Check_Pass(object sender, RoutedEventArgs e)//Function to check manager's password
        {
            //Establish a connection with the database
            using (System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection("data source= D:\\University\\Mini_Market.db"))
            {
                using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(conn))
                {
                    conn.Open(); //Open the connection to the database
                    cmd.CommandText = "SELECT passwords FROM Manager"; //Query to get the manager password from the database
                    SQLiteDataReader result = cmd.ExecuteReader();
                    string passs = "";
                    while (result.Read())
                    {
                        passs = Convert.ToString(result["passwords"]); //Store the password in a variable
                    }
                    conn.Close();//Close the connection to the database
                    if (pass.Password == passs) //Compare the password from the database to the entered password
                    {
                        Manager.Visibility = Visibility.Hidden;
                        Order.Visibility = Visibility.Visible; //Open the order menu
                        pass.Password = "";
                    }
                    else //If password is wrong
                    {
                        wrong.Visibility = Visibility.Visible; //Display a label showing that the password is incorrect
                    }
                }
            }
        }
        private void Manager_Menu(object sender, RoutedEventArgs e) //Navigate from the home menu to the Manager menu
        {
            Home.Visibility = Visibility.Hidden;
            Manager.Visibility = Visibility.Visible;
        }
        private void Order_Home(object sender, RoutedEventArgs e) //Navigate from the order menu to the home menu
        {
            Order.Visibility = Visibility.Hidden;
            Home.Visibility = Visibility.Visible;
        }
        private void Cancel(object sender, RoutedEventArgs e) //Cancel orders
        {
            //Create connection to database
            using (System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection("data source= D:\\University\\Mini_Market.db"))
            {
                using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(conn))
                {
                    conn.Open(); //Open connection to database
                    cmd.CommandText = "SELECT orderplaced FROM Items WHERE id = @IDF"; //Query to get item order status
                    cmd.Parameters.AddWithValue("@IDF", order_id.Text); //Initiate the parameter
                    SQLiteDataReader result = cmd.ExecuteReader(); //Executre the query
                    string stats = "";
                    while (result.Read())
                    {
                        stats = Convert.ToString(result["orderplaced"]); //Store the result in a variable
                    }
                    conn.Close(); //Close connection to the database
                    if ("Тrue" == stats) //Check if there is such order
                    {
                        conn.Open();
                        cmd.CommandText = @"UPDATE Items SET orderplaced = 'FALSE' WHERE id = @IDF"; //Query to update the index in the database to cancel the order
                        cmd.Parameters.AddWithValue("@IDF", order_id.Text);
                        cmd.ExecuteNonQuery();
                        Order_Done.Content = "Order canceled"; //Update the label
                        conn.Close();
                    }
                    else //If there is no such order placed
                    {
                        Order_Done.Content = "There is no such order";
                    }
                }
            }
        }
        private void Make_Order(object sender, RoutedEventArgs e) //Function to create an order
        {
            //Create connection to database
            using (System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection("data source= D:\\University\\Mini_Market.db"))
            {
                using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(conn))
                {
                    conn.Open(); //Open connection with database
                    cmd.CommandText = "SELECT orderplaced FROM Items WHERE id = @IDD"; //Query to check the status of the order
                    cmd.Parameters.AddWithValue("@IDD", order_id.Text); //Initiate parameter from text box
                    SQLiteDataReader result = cmd.ExecuteReader();
                    string stats = "";
                    while (result.Read())
                    {
                        stats = Convert.ToString(result["orderplaced"]); //Store the result in a string variable
                    }
                    conn.Close();
                    if ("False" == stats) //Check if there is already an order placed
                    {
                        conn.Open();
                        cmd.CommandText = @"UPDATE Items SET orderplaced = 'TRUE' WHERE id = @IDD"; //Query to update the index in the database
                        cmd.Parameters.AddWithValue("@IDD", order_id.Text); //Get the parameter from the text box
                        cmd.ExecuteNonQuery(); //Execute the query
                        conn.Close(); //Close the connection to the database
                        Order_Done.Content = "Item ordered"; //Update the label
                    }
                    else //If there is already an order
                    {
                        Order_Done.Content = "Item already ordered";
                    }
                }
            }
        }
        private void Manager_Home(object sender, RoutedEventArgs e) //Navigate from the manager menu to the home menu
        {
            Manager.Visibility = Visibility.Hidden;
            Home.Visibility = Visibility.Visible;
        }
#endregion
        ////////////////////////////////////////////////////////////////////////////////////// 3RD USER STORY /////////////////////////////////////////////////////////////
        #region
        private void Stock_Menu(object sender, RoutedEventArgs e) //Navigate from the home menu to the Stock check menu
        {
            Home.Visibility = Visibility.Hidden;
            Status.Visibility = Visibility.Visible;
        }
        private void Status_Home(object sender, RoutedEventArgs e) //Navigate from the check status menu to the home menu
        {
            Status.Visibility = Visibility.Hidden;
            Home.Visibility = Visibility.Visible;
        }
        private void Check_Status(object sender, RoutedEventArgs e) //Function to check the status of an item(i.e. awaiting a re-order, order placed, low on stock)
        {
            using (System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection("data source= D:\\University\\Mini_Market.db"))
            {
                using (System.Data.SQLite.SQLiteCommand cmd = new System.Data.SQLite.SQLiteCommand(conn))
                {
                    conn.Open(); //Open connection to database
                    cmd.CommandText = "SELECT * FROM Items WHERE id = @IDS"; //Query to check the status of the desired item
                    cmd.Parameters.AddWithValue("@IDS", Stock_ID.Text);
                    SQLiteDataReader result = cmd.ExecuteReader();
                    string stats = ""; //Variable to store the orderplaced value
                    int quantity = 0; //Variable to store the quantity value
                    while (result.Read())
                    {
                        stats = Convert.ToString(result["orderplaced"]);
                        quantity = Convert.ToInt32(result["qty"]);
                    }
                    conn.Close();
                    if ("True" == stats) //If order is placed update the label
                    {
                        status_I.Content = "Order is Placed";
                    }
                    else if (quantity < 0 && quantity < 5) //If quantity is below 5 but above 0
                    {
                        status_I.Content = "Low on stock";
                    }
                    else if (quantity == 0) //If there is no quantity wait for re-order
                    {
                        status_I.Content = "Awaiting re-order";
                    }
                    else //If there is no order, and the quantity is enough
                    {
                        status_I.Content = "Quantity = "+quantity;
                    }
                }
            }
        }
#endregion
    }
}
