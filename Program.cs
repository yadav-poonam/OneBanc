/**
<Summary>
Created by : Poonam Yadav
Created On : 08/11/2021
Objective : To convert different format input bank statement files into standard output file for every input file
Notes : Files which needs to be converted are passed as array of string, modification is required on the change of given input files.
        Transaction details with no specified location are market as "No Location Specified"
        Input and output files are present in main folder only.
</Summary>
        **/
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace OneBanc
{
    class Program
    {
        public static void Main(string[] args)
        {
            string[] inputFiles = { "Axis-Input-Case3.csv", "HDFC-Input-Case1.csv", "ICICI-Input-Case2.csv", "IDFC-Input-Case4.csv" };
            int NumberOfInputFiles = inputFiles.Length;
            string[] outputFiles = new string[NumberOfInputFiles];
            string[] filename;
            string inputFileName = "";
            string outputFileName = "";
            for (int fileCount = 0; fileCount < NumberOfInputFiles; fileCount++)
            {
                inputFileName = inputFiles[fileCount];
                filename = inputFileName.Split("-");
                if (filename.Length == 3 && filename[1] == "Input")
                {
                    filename[1] = "Output";
                    outputFileName = string.Join("-", filename);
                    outputFiles[fileCount] = outputFileName;
                }
                try 
                {
                    StandardizeStatement(inputFileName, outputFileName);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Please check input files format." + e.ToString());
                }
                
            }

        }
        public static void StandardizeStatement(string inputFileName, string outputFileName)
        {
            DateTime date;
            string[] sdateArray;
            string soutputDateFormat;
            int idateIndex = -1, itransactionIndex = -1, idebitAmountIndex = -1, icreditAmountIndex = -1, iamountIndex = -1;
            string stransactionType=string.Empty;
            string[] slineArray;
            double ddebit, dcredit;
            string scurrency = string.Empty, scardName = string.Empty, stransaction = string.Empty, slocation = string.Empty;
            string[] stransactionArray, samountArray;
            string sline;
            bool bisAmount = false, bisInternational = false, bisDomestic = false;
            List<string> ltransactionList;
            List<OutputFileTransactionDetails> ltransactions = new List<OutputFileTransactionDetails>();


            using (StreamReader reader = new StreamReader(inputFileName))
            {
                while ((sline = reader.ReadLine()) != null)
                {
                    slineArray = sline.Split(",");
                    int lineArraySize = slineArray.Length;
                    for (int lineColumnCounter = 0; lineColumnCounter < slineArray.Length; lineColumnCounter++)
                    {
                        if (slineArray[lineColumnCounter] == "")
                        {
                            continue;
                        }
                       
                        else if (slineArray[lineColumnCounter].ToLower().Contains("date"))
                        {
                                idateIndex = lineColumnCounter;
                                continue;
                        }
                        
                        else if (slineArray[lineColumnCounter].ToLower().Contains("debit"))
                        {
                                idebitAmountIndex = lineColumnCounter;
                                continue;

                        }
                        
                        else if (slineArray[lineColumnCounter].ToLower().Contains("credit"))
                        {
                            icreditAmountIndex = lineColumnCounter;
                             continue;

                        }

                        else if (slineArray[lineColumnCounter].ToLower().Contains("transaction description") || 
                                 slineArray[lineColumnCounter].ToLower().Contains("transaction details"))
                        {
                            itransactionIndex = lineColumnCounter;
                            continue;

                        }
                       
                        else if (sline.ToLower().Contains("domestic") || sline.ToLower().Contains("international"))
                        {
                                if (slineArray[lineColumnCounter].ToLower().Contains("domestic"))
                                {
                                    bisDomestic = true;
                                    bisInternational = false;
                                    scurrency = "INR";
                                    stransactionType = "Domestic";
                                    break;
                                }
                                if (slineArray[lineColumnCounter].ToLower().Contains("international"))
                                {
                                    bisDomestic = false;
                                    bisInternational = true;
                                    stransactionType = "International";
                                    break;
                                }
                        }
                       
                        else if (sline.ToLower().Contains("amount"))
                        {
                            bisAmount = true;
                            iamountIndex = lineColumnCounter;
                            
                        }
                        else if(lineColumnCounter == 0 && 
                            slineArray[lineColumnCounter].ToLower() != "date" && 
                            slineArray[lineColumnCounter].ToLower() != "amount" && 
                            slineArray[lineColumnCounter].ToLower() != "transaction details" && 
                            slineArray[lineColumnCounter].ToLower() != "credit" && 
                            slineArray[lineColumnCounter].ToLower() != "debit" && 
                            slineArray[lineColumnCounter].ToLower() != "domestic transactions" && 
                            slineArray[lineColumnCounter].ToLower() != "international transactions")
                        {
                            sdateArray = slineArray[idateIndex].Split('-');
                            if (Convert.ToUInt16(sdateArray[1]) > 12)
                            {
                                //date format is MM-DD-YYYY
                                soutputDateFormat = sdateArray[1] + "-" + sdateArray[0] + "-" + sdateArray[2];
                                date = DateTime.Parse(soutputDateFormat);
                                


                            }
                            else if(Convert.ToUInt16(sdateArray[1])<= 12 && Convert.ToUInt16(sdateArray[2]) <= 19)
                            {
                                //date fromat is DD-MM-YY
                                soutputDateFormat = sdateArray[0] + '-' + sdateArray[1] + '-' + "20" + sdateArray[2];
                                date = DateTime.Parse(soutputDateFormat);

                                
                            }
                            else
                            {
                                //initially considering date format as DD-MM-YYYY
                                soutputDateFormat = slineArray[idateIndex];
                                date = DateTime.Parse(soutputDateFormat);
                                
                            }
                            stransactionArray = slineArray[itransactionIndex].Split(' ');
                            if (bisInternational)
                            {
                                 int ilastIndex = stransactionArray.Length - 1;
                                int icurrencyIndex = ilastIndex;
                                scurrency = stransactionArray[ilastIndex];
                                ilastIndex--;
                                for (int i = stransactionArray.Length - 2; i >= 0; i--)
                                {
                                    if (!string.IsNullOrEmpty(stransactionArray[i]))
                                    {
                                        ilastIndex = i;
                                        break;
                                    }
                                }
                                slocation = stransactionArray[ilastIndex];
                                if (Double.TryParse(slocation, out double slocationDouble))
                                {
                                    slocation = "No Location Specified";
                                }
                                ltransactionList = new List<string>(stransactionArray);
                                ltransactionList.RemoveAt(icurrencyIndex);
                                stransaction = string.Join(" ", ltransactionList);

                            }

                            if(bisDomestic)
                            {
                                int ilastIndex = stransactionArray.Length - 1;
                                for (int i = stransactionArray.Length - 1; i >= 0; i--)
                                {
                                    if (!string.IsNullOrEmpty(stransactionArray[i]))
                                    {
                                        ilastIndex = i;
                                        break;
                                    }
                                    
                                }
                                slocation = stransactionArray[ilastIndex];
                                if (Double.TryParse(slocation, out double slocationDouble))
                                {
                                    slocation = "No Location Specified";
                                }
                                stransaction = string.Join(" ", stransactionArray);
                            }

                            if (bisAmount)
                            {
                                samountArray = slineArray[iamountIndex].Split(' ');
                                if (samountArray.Length<=1)
                                {
                                    dcredit = 0;
                                    ddebit = double.Parse(slineArray[iamountIndex]);
                                }
                                else
                                {
                                    ddebit = 0;
                                    string[] amountColumns = slineArray[iamountIndex].Split(' ');
                                    dcredit = double.Parse(amountColumns[0]);
                                }
                            }
                            else
                            {
                                if (slineArray[idebitAmountIndex]!=string.Empty)
                                {
                                    dcredit = 0.00;
                                    ddebit = double.Parse(slineArray[idebitAmountIndex]);
                                }
                                else
                                {
                                    ddebit = 0.00;
                                    dcredit = double.Parse(slineArray[icreditAmountIndex]);
                                }

                            }

                            ltransactions.Add(new OutputFileTransactionDetails(date,stransaction, ddebit, dcredit, scurrency, scardName, stransactionType, slocation));
                            break;

                        }
                        else
                        {
                            scardName = slineArray[lineColumnCounter];
                        }
                    }
                }
               
            }
            using (StreamWriter strwrt = new StreamWriter(outputFileName))
            {
                strwrt.WriteLine("Date, Transaction Description, Debit, Credit, Currency, CardName,Transaction, Location");
                for (int ilistTransactionsCounter = 0; ilistTransactionsCounter < ltransactions.Count; ilistTransactionsCounter++)
                {
                    Console.WriteLine(ltransactions[ilistTransactionsCounter].getString());
                    strwrt.WriteLine(ltransactions[ilistTransactionsCounter].getString());
                }

            }

        }
    }
}
