using System;
using System.IO;
using System.Collections.Generic;

namespace ReformatSelfConfiguredDIP
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string FolderIn = args[0];

                try
                {
                    Directory.GetFiles(FolderIn);
                }
                catch
                {
                    Console.WriteLine("Folder does not exist:" + FolderIn);
                    Environment.Exit(1);
                }
                
                string[] files = Directory.GetFiles(FolderIn, "*.txt", SearchOption.TopDirectoryOnly);

                foreach (string file in files)
                {
                    List<string> outlines = new List<string>();

                    string[] lines = File.ReadAllLines(file);

                    string filenum = file.Substring(file.LastIndexOf("-") + 1);
                    string filepath = FolderIn + @"\" + "DIPIndex-" + filenum;

                    string notetext1 = "";
                    string notetext2 = "";
                    string notetextline = "";
                    string noteflagline = "";
                    string notetitleline = "";
                    string note = "";
                    string oldvalue = "";
                    string reporttitle = "";
                    string reportnumber = "";
                    string reportname = "";
                    string reportsecurity = "";

                    foreach (string line in lines)
                    {
                        if (line.Contains("DocTypeName") is true)
                        {

                            oldvalue = (line.Substring(line.IndexOf(":") + 1)).Trim();
                            reportname = oldvalue.Trim();                            

                            if (oldvalue.Contains("_") is true || oldvalue.Contains("-") is true)
                            {
                                reporttitle = oldvalue.Substring(oldvalue.IndexOf(" ")).Trim();
                                reportnumber = oldvalue.Substring(0, oldvalue.IndexOf(" ")).Trim();
                            }
                            else
                            {
                                reporttitle = oldvalue;
                                reportnumber = oldvalue;
                            }

                            if (reportnumber.Contains("BI_ISOA") is true || reportnumber.Contains("EM_AUTH") is true || reportnumber.Contains("EM_LIST") is true || reportnumber.Contains("EM_SUPR") is true || reportnumber.Contains("EM_TRANS") is true || reportnumber.Contains("LN_INSID") is true || reportnumber.Contains("MS_AAPPL") is true || reportnumber.Contains("MS_AREPT") is true || reportnumber.Contains("MM_ULOCK") is true || reportnumber.Contains("MM_ULOG") is true)
                            {
                                reportsecurity = "Y";
                            }
                            else
                            { 
                                reportsecurity = "N"; 
                            }

                            outlines.Add(">>DocTypeName: Roselle Report Legacy");
                            outlines.Add("Report Title: " + reporttitle);
                            outlines.Add("Report Number: " + reportnumber);
                            outlines.Add("Report Name: " + reportname);
                            outlines.Add("Report Security: " + reportsecurity);
                        }
                        else
                        {
                            if (line.Contains("DocDate") is true)
                            {
                                DateTime docdate = Convert.ToDateTime(line.Substring(line.IndexOf(":") + 2).Trim());
                                DateTime retentiondateend = docdate.AddYears(7);
                                string date = Convert.ToString(docdate);
                                string retdate = Convert.ToString(retentiondateend);

                                outlines.Add(">>DocDate: " + date.Substring(0, date.LastIndexOf("/") + 5));
                                outlines.Add("Retention Date End: " + retdate.Substring(0, retdate.LastIndexOf("/") + 5));
                            }
                            else
                            {
                                if (line.Contains("Document Handle") is false && line.Contains("DiskgroupNum") is false && line.Contains("VolumeNum") is false)
                                {
                                    if (line.Contains("NoteUserName") is true)
                                    {
                                        outlines.Add(">>NoteUserName: MANAGER");
                                    }
                                    else
                                    {                                        
                                        if (line.Contains("NoteText") is true || line.Contains("NoteFlag") is true || line.Contains("NoteTitle") is true)
                                        {                                                                                      
                                            if (line.Contains("NoteText") is true)
                                            {
                                                notetext1 = line.Substring(0, line.IndexOf(" ")).Trim();
                                                notetext2 = line.Substring(line.IndexOf(":") + 2).Trim();
                                            }
                                            else
                                            {
                                                if (line.Contains("NoteFlag") is true)
                                                {
                                                    noteflagline = line;                                                  
                                                }
                                                else
                                                {
                                                    if (line.Contains("NoteTitle") is true)
                                                    {
                                                        notetitleline = line;
                                                        note = line.Substring(line.IndexOf(":") + 2).Trim();
                                                        notetextline = notetext1.Trim() + " " + note.Trim() + ": " + notetext2.Trim();

                                                        outlines.Add(notetextline.Trim());
                                                        outlines.Add(noteflagline.Trim());
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (line.Contains("Account #") is true)
                                            {
                                                if (reporttitle.Contains("Loan") is true)
                                                {
                                                    outlines.Add(line.Replace("Account #", "Loan Number"));
                                                }
                                                else
                                                {
                                                    outlines.Add(line.Replace("Account #", "Deposit Number"));
                                                }
                                            }
                                            else
                                            {
                                                if (line.Contains("Customer Name") is true)
                                                {
                                                    if (reporttitle.Contains("Loan") is true)
                                                    {
                                                        outlines.Add(line.Replace("Customer Name", "Loan Name"));
                                                    }
                                                    else
                                                    {
                                                        outlines.Add(line.Replace("Customer Name", "Deposit Name"));
                                                    }
                                                }
                                                else
                                                {
                                                    outlines.Add(line.Trim());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    string output = string.Join(Environment.NewLine, outlines.ToArray());
                    File.AppendAllText(filepath, output.Trim());
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Environment.Exit(1);
            }
        }
    }
}