using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.Emit;
using System.Windows;
using Dapper;

namespace VirtualDisplay
{
    // class works with DB SQLite
    public class SqliteDataAccess
    {

        // Return path of SetupDB buttons
        private string LoadConnectionStringDB()
        {
            //название таблицы SetupDB - таблица для настройки, 
            string setupID = "db";
            //return ConfigurationManager.ConnectionStrings[setupID].ConnectionString;
            //return ConfigurationManager.ConnectionStrings[setupID].ConnectionString.Replace("%PROGRAMDATA%", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            return ConfigurationManager.ConnectionStrings[setupID].ConnectionString.Replace("%PROGRAMDATA%", Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
        }

        // Save a button to SetupDB
        public void SaveButtonInSetup(SaveButtonModel b)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                try
                {
                    //cnn.Execute("insert into SetupButtonTable (BtnId, BtnName, BtnDate, BtnFunction) values (@BtnId, @BtnName, @BtnDate, @BtnFunction);", b);
                    cnn.Execute("insert into SetupButtonTable (BtnId, BtnName, BtnDate, BtnFunction) values (@BtnId, @BtnName, @BtnDate, @BtnFunction);", b);                
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Не удается записать кнопку в базу данных. Код ошибки #1005" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // return a List of SetupDB
        public List<SaveButtonModel> ShowSetupButtons()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {                
                try
                {
                    var output = cnn.Query<SaveButtonModel>("SELECT * FROM SetupButtonTable ORDER BY BtnDate DESC", new DynamicParameters());
                    //var output = cnn.Query<SaveButtonModel>("SELECT BtnDate FROM SetupButtonTable", new DynamicParameters());
                    return output.ToList();
                }
                catch
                {
                    MessageBox.Show("Отсутствует доступ к базе данных для настройки оборудования. Код ошибки #1001.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }                
            }
        }

        //возвращает список нажитий в установленный период времени. 0 - час, 1 - день, 2 - месяц, 3 - год
        public List<CountButtons> MethodCountButtons(String bg, String end, int selection)            
        {
            List<CountButtons> output = new List<CountButtons>();
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                String cmd="";

                switch (selection)
                {
                    //число нажатий за один день по часам
                    case 0:
                        cmd = "select count(strftime('%H', BtnDate)), strftime('%H', BtnDate) FROM ButtonTable WHERE strftime('%Y-%m-%d', BtnDate) BETWEEN '" + bg + "' and '" + end + "' GROUP by strftime('%H', BtnDate)";
                        break;
                    //число нажатий за один месяц по дням
                    case 1:
                        cmd = "select count(strftime('%d', BtnDate)), strftime('%Y-%m-%d', BtnDate) FROM ButtonTable WHERE strftime('%Y-%m-%d', BtnDate) BETWEEN '" + bg + "' and '" + end + "' GROUP by strftime('%d', BtnDate)";
                        break;
                    //число нажатий за один год по месяцам
                    case 2:
                        cmd = "select count(strftime('%m', BtnDate)), strftime('%Y-%m', BtnDate) FROM ButtonTable WHERE strftime('%Y-%m-%d', BtnDate) BETWEEN '" + bg + "' and '" + end + "' GROUP by strftime('%m', BtnDate)";
                        break;
                    //числo нажатий по годам
                    case 3:
                        cmd = "select count(strftime('%m', BtnDate)), strftime('%Y', BtnDate) FROM ButtonTable WHERE strftime('%Y-%m-%d', BtnDate) BETWEEN '" + bg + "' and '" + end + "' GROUP by strftime('%y', BtnDate)";
                        break;
                }                    
                
                IDataReader dr = cnn.ExecuteReader(cmd);

                while (dr.Read())
                {
                    CountButtons cb = new CountButtons();
                    cb.btn_count = dr.GetInt32(0);
                    cb.btn_date = dr.GetString(1);
                    output.Add(cb);
                }
                dr.Close();

                return output.ToList();
            }

        }

        public String FindButtonName(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                String output = cnn.QuerySingleOrDefault<String>("SELECT BtnName FROM SetupButtonTable WHERE BtnID = '" + id + "'", new DynamicParameters());
                return output;
            }

        }

        public String FindButtonType(int id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                string output = cnn.QuerySingleOrDefault<String>("SELECT BtnFunction FROM SetupButtonTable WHERE BtnID = '" + id + "'", new DynamicParameters());
                return output;
            }
        }

        // return a List of Buttons DB
        public List<ButtonModel> ShowButtons(string bg, string end)
        {
            List<ButtonModel> bm = new List<ButtonModel>();
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                var output1 = cnn.Query<ButtonModel>("SELECT * FROM ButtonTable WHERE strftime('%Y-%m-%d',BtnDate) BETWEEN '" + bg + "' and '" + end + "' ORDER BY BtnDate DESC", new DynamicParameters());
                //var output1 = cnn.Query<ButtonModel>("SELECT * FROM ButtonTable WHERE strftime('%Y-%m-%d',BtnDate) BETWEEN '" + bg + "' and '" + end + "' AND BtnStatus='active' ORDER BY BtnDate DESC", new DynamicParameters());
                //var output1 = cnn.Query<ButtonModel>("SELECT * FROM ButtonTable WHERE strftime('%Y.%m.%d',BtnDate) BETWEEN '" + bg + "' and '" + end + "' ORDER BY BtnDate DESC", new DynamicParameters());

                return output1.ToList();
            }
        }


        // delete a button with ID from SetUpDB 
        public void DeleteButton(int btnId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                //cnn.Execute("DELETE FROM ButtonTable WHERE BtnId = '"+btnId.ToString()+"';");
                cnn.Execute($"DELETE FROM SetupButtonTable WHERE BtnId = '{ btnId }'");
            }
        }

        // delete all buttons from SetUpDB 
        public void DeleteAllButtons()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                //cnn.Execute("DELETE FROM ButtonTable WHERE BtnId = '"+btnId.ToString()+"';");
                cnn.Execute($"DELETE FROM SetupButtonTable");
            }
        }

        // delete all buttons from DB 
        public void DeleteAllButtonsFromCallingBD()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                //cnn.Execute("DELETE FROM ButtonTable WHERE BtnId = '"+btnId.ToString()+"';");
                cnn.Execute($"DELETE FROM ButtonTable");                
            }
        }

        // delete all buttons from SetUpDB 
        public void DeleteAllButtonsFromSetupBD()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                //cnn.Execute("DELETE FROM ButtonTable WHERE BtnId = '"+btnId.ToString()+"';");
                cnn.Execute($"DELETE FROM SetupButtonTable");
            }
        }

        // Функция проверяет, если уже кнопка с таким ID производителяв БД. Возвращает True, если кнопка есть в БД. False, если кнопка отсутствует в БД.
        public bool CheckID_btnInTable(int btn_id)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                try
                {
                    cnn.Open();
                    int chckBtn = cnn.QuerySingleOrDefault<int>($"SELECT BtnId FROM SetupButtonTable WHERE BtnId = '{ btn_id }'");
                    cnn.Close();

                    if (chckBtn > 0)
                        return true;
                    else
                        return false;
                }
                catch
                {
                    MessageBox.Show("Отсутствует доступ к базе данных для настройки оборудования. Код ошибки #1002.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true;                
                }
            }
        }

        public string GetFunction_btnInTable(int btn_id)
        {
            String btn_function;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                try
                {
                    cnn.Open();
                    btn_function = cnn.QuerySingleOrDefault<String>($"SELECT BtnFunction FROM SetupButtonTable WHERE BtnId = '{ btn_id }'");
                    cnn.Close();
                    return btn_function;
                }
                catch
                {
                    MessageBox.Show("Отсутствует доступ к базе данных для настройки оборудования. Код ошибки #1007.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }

        }

        public String GetName_btnInTable(int btn_id)
        {
            String btn_name;
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                try
                {
                    cnn.Open();
                    btn_name = cnn.QuerySingleOrDefault<String>($"SELECT BtnName FROM SetupButtonTable WHERE BtnId = '{ btn_id }'");
                    cnn.Close();
                    return btn_name;
                }
                catch
                {
                    MessageBox.Show("Отсутствует доступ к базе данных для настройки оборудования. Код ошибки #1003.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
        }

        public SaveButtonModel Get_FromTable(int btn_id)
        {
            SaveButtonModel b = new SaveButtonModel();
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                try
                {
                    cnn.Open();
                    b = cnn.QuerySingleOrDefault<SaveButtonModel>($"SELECT * FROM SetupButtonTable WHERE BtnId = '{ btn_id }'");
                    cnn.Close();
                    return b;
                }
                catch
                {
                    MessageBox.Show("Отсутствует доступ к базе данных для настройки оборудования. Код ошибки #1006.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
        }

        // Функция проверяет, если уже кнопка с таким именем в БД. Возвращает True, если кнопка есть в БД. False, если кнопка отсутствует в БД.
        public bool CheckName_btnInTable(string btn_name)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                try
                {
                    cnn.Open();
                    int chckBtn = cnn.QuerySingleOrDefault<int>($"SELECT BtnName FROM SetupButtonTable WHERE BtnName = '{ btn_name }'");
                    cnn.Close();

                    if (chckBtn > 0)
                        return true;
                    else
                        return false;
                }
                catch
                {
                    MessageBox.Show("Отсутствует доступ к базе данных для настройки оборудования. Код ошибки #1004.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true;
                }
            }
        }

        // Save a button to ButtonDB
        public void SaveButtonsDisplay(ButtonModel btn)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                if (btn.BtnStatus == Constants.SERVED_STATUS) // вызов отменен
                {
                    String hourAgo = DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss");

                    // утанавливаем в BtnStatus='served' (отмена) в последней записи за предыдущий час
                    cnn.Execute($"UPDATE ButtonTable SET BtnStatus='served' WHERE Id= (SELECT Id FROM ButtonTable WHERE BtnId='{ btn.BtnId }' AND strftime('%Y-%m-%d', BtnDate) > '{ hourAgo }'  ORDER  BY BtnDate DESC LIMIT 1)");

                    //cnn.Execute("insert into ButtonTable (BtnId, BtnName, BtnDate, BtnFunction, BtnStatus) values (@BtnId, @BtnName, @BtnDate, @BtnFunction, @BtnStatus);", btn);
                }
                else
                {
                    cnn.Execute("INSERT INTO ButtonTable (BtnId, BtnName, BtnDate, BtnFunction, BtnStatus) VALUES (@BtnId, @BtnName, @BtnDate, @BtnFunction, @BtnStatus);", btn);
                }
            }
        }

        public bool ChangeEventStatusByBtnName(string BtnName, string new_status)
        {
            try
            {
                using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
                {
                    //cnn.Execute("insert into ButtonTable (BtnId, BtnName, BtnDate, BtnFunction) values (@BtnId, @BtnName, @BtnDate, @BtnFunction);", btn);
                    cnn.Execute($"Update ButtonTable SET BtnStatus = '{ new_status }' WHERE BtnName = '{ BtnName }'");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Не удалось изменить статус вызова. Код ошибки #14122020-2342.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        //возращает true, если кнопки не настроены в БД
        public bool IsEmpty()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionStringDB()))
            {
                try
                {
                    cnn.Open();
                    int count = cnn.QuerySingleOrDefault<int>($"SELECT count(*) FROM SetupButtonTable");
                    cnn.Close();

                    if (count > 0)
                        return false;
                    else
                        return true;
                }
                catch
                {
                    MessageBox.Show("Отсутствует доступ к базе данных для настройки оборудования. Код ошибки #1004.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true;
                }
            }
        }
    }
}
