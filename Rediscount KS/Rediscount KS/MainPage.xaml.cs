using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using static Rediscount_KS.MainPage;


namespace Rediscount_KS
{

    public partial class MainPage : ContentPage
    {
        string auditorName;
        string departmentofDestignation;
        public ObservableCollection<Auditor> Allaudits { get; set; } = new ObservableCollection<Auditor>();
        public ObservableCollection<Equipment> AllItems { get; set; } = new ObservableCollection<Equipment>();
        public ObservableCollection<Equipment> Scanned{ get; set; } = new ObservableCollection<Equipment>();
        public MainPage()
        {
            InitializeComponent();
            
            BindingContext = this;
            try
            {
                string PathToInventory = File.ReadAllText(FileSystem.AppDataDirectory + "pathtoinvebtory.txt");
                if (PathToInventory =="")
                {
                    DisplayAlert("Пробачте", "Жодного файла інвентаризації не знайдено, спобуйте Завантажити у меню.", "OK");
                }
                else
                {
                    LoadFiles(PathToInventory);                 
                    DisplayAlert("Файл інвентаризації завантажено", PathToInventory, "OK");
                }              
            }
            catch
            {
                DisplayAlert("Пробачте", "Жодного файла комісії не знайдено, спобуйте Завантажити у меню.", "OK");
            }
            try
            {
                string PathToAudits = File.ReadAllText(FileSystem.AppDataDirectory + "pathtoaudits.txt");
                if (PathToAudits == "")
                {
                    DisplayAlert("Пробачте", "Жодного файла комісії не знайдено, спобуйте Завантажити у меню.", "OK");
                }
                else
                {
                    LoadAudits(PathToAudits);
                   
                }
                if (Allaudits.Count>0)
                {
                    List <string> auditorss = new List<string>();
                    for (int i = 0; i < (Allaudits.Count - 1); i++)
                    {
                        auditorss.Add(Allaudits[i].AuditName+"\n "+Allaudits[i].NameOfAuditor + "\n " + Allaudits[i].WhereShouldAuditorGo + "\n ");
                        
                    }
                    Task.Run(async () => 
                    {

                        string[] stringsSS = auditorss.ToArray();
                        string aaa = await DisplayActionSheet("виберіть комісію", null, null, stringsSS);
                        for (int i = 0; i < stringsSS.Length-1; i++)
                        {
                            if (aaa == stringsSS[i])
                            {
                                auditorName = Allaudits[i].NameOfAuditor + " " + Allaudits[i].AuditName;
                                departmentofDestignation = Allaudits[i].WhereShouldAuditorGo;
                            }
                        }


                    });
                   
                }
                
            }
            catch
            {
                DisplayAlert("Пробачте", "Жодного файла коміссії не знайдено, спобуйте Завантажити у меню.", "OK");
            }
            

        }
        public class Equipment
        {
            public string InventarNumber { get; set; }
            public string NameOfItem { get; set; }
            public string DepartmentOfDestignation { get; set; }
            public string DepartmentFound { get; set; }
            public string SerialNumber { get; set; }
            public string HeadOfAudit { get; set; }
            public string Comment { get; set; }
            public string Scanned { get; set; }

        }
        public class Auditor
        {
            public string NameOfAuditor { get; set; }
            public string AuditName { get; set; }
            public string WhereShouldAuditorGo { get; set; }

        }

        public async void MenuButton_Clicked(object sender, EventArgs e)
        {
            string menuaction = await DisplayActionSheet(null,null,null,"Додати по серійнику","Завантажити(Змінити) файл коміссій","Завантажити(Змінити) файл інвентаризації","Зберегти і завершити інвентаризацію" );
           
            if (menuaction == "Додати по серійнику")
            {

                string result2 = await DisplayPromptAsync("Введіть серійний номер", null, "шукати");
                try
                {
                    var older1List = new List<Equipment>();
                    var indx = AllItems.First(i => i.SerialNumber == result2);
                    int indexx = AllItems.IndexOf(indx);
                    AllItems[indexx].DepartmentFound = departmentofDestignation;
                    AllItems[indexx].HeadOfAudit = auditorName;
                    if (AllItems[indexx].Scanned == "yes")
                    {
                       
                            string result3 = await DisplayPromptAsync("Цей номер вже вносили", "Підкажіть чого так сталося", "зберегти коментар");
                            AllItems[indexx].Comment = result3;

                       
                    }
                    AllItems[indexx].Scanned = "yes";


                    Equipment eq1 = new Equipment();
                    eq1 = AllItems[indexx];

                    older1List = (Scanned).ToList<Equipment>();
                    Scanned.Clear();
                    Scanned.Add(eq1);
                    older1List.ForEach(x => Scanned.Add(x));

                }
                catch
                {

                    {
                        DisplayAlert("Cталася помилка", "Чого? Чому? Навіщо? Відповідей на це питання ми не дізнаємось ніколи, але такого серійника я в списку не знайшов", "Ну і не особливо хотілося");
                    }
                }



            }
            if (menuaction == "Завантажити(Змінити) файл коміссій")
            {
                try
                {
                    var res = await FilePicker.PickAsync();
                    string PathOfAuditsTosave = res.FullPath;
                    LoadAudits(PathOfAuditsTosave = res.FullPath);
                    File.WriteAllText(FileSystem.AppDataDirectory + "pathtoaudits.txt", PathOfAuditsTosave);
                }
                catch
                {
                    await DisplayAlert("Пробачте", "Файл не завантажено.", "OK");
                };
            }
            if (menuaction == "Завантажити(Змінити) файл інвентаризації")
            {
                try
                {
                    var result = await FilePicker.PickAsync();                   
                    string PathOfInventoryToSave = result.FullPath;
                    LoadFiles(PathOfInventoryToSave);
                    File.WriteAllText(FileSystem.AppDataDirectory + "pathtoinvebtory.txt", PathOfInventoryToSave);
                }
                catch
                {
                    await DisplayAlert("Пробачте", "Файл не завантажено.", "OK");
                };  
            }
            if (menuaction == "Зберегти і завершити інвентаризацію")               
            {
                string Alllines = "";
                for (int i = 0; i < AllItems.Count-1; i++)
                {
                    string linetosave = AllItems[i].InventarNumber +";"+ AllItems[i].NameOfItem + ";" + AllItems[i].DepartmentOfDestignation + ";" + AllItems[i].DepartmentFound + ";" + AllItems[i].SerialNumber + ";" + AllItems[i].HeadOfAudit + ";" + AllItems[i].Comment + ";" + AllItems[i].Scanned + "\n";
                    Alllines = Alllines + linetosave;
                }
                
                string PathToSaveThisFile = File.ReadAllText(FileSystem.AppDataDirectory + "pathtoinvebtory.txt");
                File.WriteAllText(PathToSaveThisFile, Alllines);
                
                await DisplayAlert("Файл інвентаризації збережено", PathToSaveThisFile, "OK");
            }

        }
        public void LoadFiles(string pathes)
        {    
          var enumLines = File.ReadLines(pathes, Encoding.UTF8);
                foreach (var line in enumLines)
                {
                    string[] parts = line.Split(';');
                    Equipment equipment = new Equipment();
                    equipment.InventarNumber = parts[0];
                    equipment.NameOfItem = parts[1];
                    equipment.DepartmentOfDestignation = parts[2];
                    equipment.DepartmentFound = parts[3];
                    equipment.SerialNumber = parts[4];
                    equipment.HeadOfAudit = parts[5];
                    equipment.Comment = parts[6];
                    equipment.Scanned = parts[7];

                    AllItems.Add(equipment);
                }               
           
        }
        public void LoadAudits(string pathes) 
        {
            var enumLines = File.ReadLines(pathes, Encoding.UTF8);
            foreach (var line in enumLines)
            {
                string[] AUDITS = line.Split(';');
                Auditor auditor= new Auditor();
                auditor.AuditName = AUDITS[0];
                auditor.NameOfAuditor= AUDITS[1];
                auditor.WhereShouldAuditorGo = AUDITS[2];
                

                Allaudits.Add(auditor);
            }

        }

        private async void theeditor_Completed(object sender, EventArgs e)
        {
            try
            {
                String scannednumber = theeditor.Text;
                char[] chars = scannednumber.ToCharArray();
                string realnumber = "";
                for (int i = 0; i < chars.Length - 2; i++)
                {
                    realnumber = realnumber + chars[i];
                }


                try
                {
                    var olderList = new List<Equipment>();
                    var indx = AllItems.First(i => i.InventarNumber == realnumber);
                    int indexx = AllItems.IndexOf(indx);
                    AllItems[indexx].DepartmentFound = departmentofDestignation;
                    AllItems[indexx].HeadOfAudit = auditorName;
                    if (AllItems[indexx].Scanned == "yes")
                    {
                        
                            string result1 = await DisplayPromptAsync("Цей номер вже вносили", "Підкажіть чого так сталося", "зберегти коментар");
                            AllItems[indexx].Comment = result1;
                        
                        
                    }
                    AllItems[indexx].Scanned = "yes";


                    Equipment eq1 = new Equipment();
                    eq1 = AllItems[indexx];

                    olderList = (Scanned).ToList<Equipment>();
                    Scanned.Clear();
                    Scanned.Add(eq1);
                    olderList.ForEach(x => Scanned.Add(x));

                }
                catch
                {

                    {
                        DisplayAlert("Cталася помилка", "Чого? Чому? Навіщо? Відповідей на це питання ми не дізнаємось ніколи, але такого серійника я в списку не знайшов", "Ну і не особливо хотілося");
                    }
                }

                theeditor.Text = "";
            }
            catch { }

        
        }

    }
}
