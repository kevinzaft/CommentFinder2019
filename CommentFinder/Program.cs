using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace CommentFinder
{
    /// <summary>
    /// This class is the main class for the program. All the computation, methods and I/O happens here
    /// </summary>
    class Program
    {
        /// <summary>
        /// returns a boolean value of whether or not the input string contains 'todo'
        /// </summary>
        private bool checkToDo(string line)
        {
            return line.ToLower().Contains("todo");
        }

        /// <summary>
        /// The Main method of the program
        /// </summary>
        static void Main(string[] args)
        {
            var fileLocations = args; //variable for storing the file addresse(s)
            var lookUps = new List<LookUp>();//variable for storing the lookups for file type and comment

            //getting the lookup values from json and storing them in lookUps variable
            using (StreamReader r = new StreamReader(("data.json")))
            {
                string json = r.ReadToEnd();
                lookUps = JsonConvert.DeserializeObject<List<LookUp>>(json.ToString());
            }

            //manually getting file paths from users if not specified in args
            if (args.Length < 1)
            {
                Console.WriteLine("Please Specify the full path and name(with extension) of the file you wish to check-in.");
                Console.WriteLine("For multiple files, please separate them with a space.");
                fileLocations = Console.ReadLine().Split(" ");
            }

            //parsing the files for comments
            foreach(var location in fileLocations)
            {
                var fileInfo = new FileInfo(location);
                var language = lookUps.Where(x => x.FileExtension.Equals(fileInfo.Extension));

                //ignoring files without a name or extension
                if (fileInfo.Extension.Equals("") || Path.GetFileNameWithoutExtension(fileInfo.FullName).Equals(""))
                {
                    Console.WriteLine("The file: " + fileInfo.FullName + " either does not have a filename or does not have an extension; this file will be ignored");
                    continue;
                }
                //if the file extension is not found in the data.json file, notify the user
                else if (language.Count() <= 0 )
                {
                    Console.WriteLine("The file extension: " + fileInfo.Extension + " does not seem to be in our lookup. This file will be skipped");
                    Console.WriteLine("Please add the file type to the data.json file along with it's commenting methods");
                }
                //parse the file for comments
                else
                {
                    var totalLines = 0;
                    var singlelineComments = 0;
                    var linesOfBlockComments = 0;
                    var numberOfBlockComments = 0;
                    var toDos = 0;
                    var inCodeBlock = false;

                    //create a streamReader for the file at 'location'
                    var streamReader = new StreamReader(location);
                    var line = "";
                    //reading the file line by line
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        totalLines++;

                        if (inCodeBlock) //checking to see if we are in a comment block
                        {
                            
                            if (line.Contains(language.First().MultilineCommentEnd)) //checking if the line is the end of a comment block
                            {
                                linesOfBlockComments++;
                                inCodeBlock = false;
                            }
                            else if (line.Contains(language.First().MultilineCommentMiddle)) //checking if the line is the middle of a comment block
                            {
                                linesOfBlockComments++;
                            }

                        }
                        else
                        {
                            if (line.Contains(language.First().SingleLineComment)) //checking if the line has a single line comment
                            {
                                singlelineComments++;
                            }
                            else if (line.Contains(language.First().MultilineCommentStart)) //checking if the line is the start of a comment block
                            {
                                numberOfBlockComments++;
                                linesOfBlockComments++;
                                inCodeBlock = true;
                            }
                        }
                            
                        if (line.ToLower().Contains("todo")) //checking if the line is 
                        {
                            toDos++;
                        }
                    }
                    
                    Console.WriteLine("For file: " + location);
                    Console.WriteLine("Total # of lines: " + totalLines);
                    Console.WriteLine("Total # of comment lines: " + (singlelineComments+ linesOfBlockComments));
                    Console.WriteLine("Total # of single line comments: " + singlelineComments);
                    Console.WriteLine("Total # of comment lines within block comments: " + linesOfBlockComments);
                    Console.WriteLine("Total # of block line comments: " + numberOfBlockComments);
                    Console.WriteLine("Total # of TODO's: " + toDos);
                    Console.WriteLine("Press any key to continue.");
                    Console.ReadKey();
                }
            }
        }
    }
}
