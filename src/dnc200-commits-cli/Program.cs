/**
 * FileName: Program.cs
 * Author: Kenneth Lapid David
 *
 * This is a file that contains the Program class which runs a program
 * that will return the number of commits made by a github user or a 
 * github repository in the past 90 days
 */

using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace dnc200_commits_cli
{
    public class Program
    {
        private static String URL = "https://api.github.com/";
        private static String USER = "users/";
        private static String REPO = "repos/";

        public const int TIMEFRAME = 38;

        public const int WEEKS = 52;

        /**
         * This method is the bulk of the interactive part of our program. It will prompt the user for correct input
         * and ultimately return the number of commits upon a successful run, or it will tell the user what went
         * wrong if an error occurred.
         *
         * @param args: An array of any input arguments
         */
        public static void Main(string[] args)
        {
            String response = "";
            String userName = "";
            String repoName = "";
            Boolean valid = false;
            int commits = 0;

            //hint: this is the base of the github API url "https://api.github.com/"
            //capture user input to determine if they want to search a user or a repository
            Console.WriteLine("Welcome! This program retrieves the amount of commits made from a user or a repository within the past 90 days.");
            Console.WriteLine("Please specify if you would like to search by 'user' or 'repository': ");

            // Keep asking for user input until they type in a valid response
            do {
                response = Console.ReadLine();
                if(String.Equals(response,"user",StringComparison.OrdinalIgnoreCase) || String.Equals(response,"repository",StringComparison.OrdinalIgnoreCase)) {
                    valid = true;
                } else {
                    Console.WriteLine("Invalid Input. Please type in either 'user' to search user commits or 'repository' to search for repo commits");
                }       
            } while(!valid);

            // Now get the commits based on if they selected the user or repository
            if(String.Equals(response,"user",StringComparison.OrdinalIgnoreCase)) {
                Console.WriteLine("You selected 'user'");
                Console.WriteLine("Please enter the username:");
                userName = Console.ReadLine().ToLower();
                commits = Program.Request(userName);
            } else {
                Console.WriteLine("You selected 'repository'");
                Console.WriteLine("Please enter the name of the repository:");
                repoName = Console.ReadLine();
                Console.WriteLine("Please enter the username of the owner of this repository:");
                userName = Console.ReadLine().ToLower();
                commits = Program.Request(userName,repoName,false);
            }

            // If all input was valid, return the number of commits for the specified type 
            if(commits > -1) {
                if(String.Equals(response,"user",StringComparison.OrdinalIgnoreCase)) {
                    Console.WriteLine("There was a total of " + commits + " commits made by the user: " + userName + " in the last 90 days");
                } else {
                    Console.WriteLine("There was a total of " + commits + " commits made in the repository: " + repoName + " in the last 90 days");
                }
            }  
            
        }

        // Write methods to make a reqest to the Github api

        /**
         * This method will make a request to the github api using repository information
         * to retrieve commit data and return the number of commits made to the repo within
         * the past 90 days (13 weeks) 
         *
         * @param userName: The github username of the creator of this repository
         * @param repoName: The name of the repository
         * @return the number of commits made to the repo within the past 90 days
         */
        public static int Request(String userName, String repoName, Boolean owner) {
            // Use a try catch to handle any server errors
            try {
                // 'using' will be used to ensure proper closure of web server after we are done using it
                using(WebClient client = new WebClient()) {
                    int numCommits = 0;
                    client.BaseAddress = URL + REPO; // Create a base address 
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36"); // Add this on for authentication
                    String json = client.DownloadString(userName + "/" + repoName + "/stats/participation"); // Enter in the remaining parts of the URL and get the JSON data
                    AllCommits commits = JsonConvert.DeserializeObject<AllCommits>(json); // Convert json data into a list

                    // Iterate through the json data to find the commits
                    if(owner) {
                        for(int i = WEEKS - 1; i > TIMEFRAME; i--) {
                            numCommits += commits.Owner[i];
                        }
                    } else {
                        for(int i = WEEKS - 1; i > TIMEFRAME; i--) {
                            numCommits += commits.All[i];
                        }
                    }

                    return numCommits;
                }
            } catch(Exception ex) {
                // Send through exception message to let user know what went wrong
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /**
         * This method will make a request to the github api using user information
         * to retrieve commit data and return the number of commits made by the user within
         * the past 90 days (13 weeks) 
         *
         * @param userName: The github username of the creator of this repository
         * @return the number of commits made to the repo within the past 90 days
         */
        public static int Request(string userName) {
            // use a try catch to handle any server errors
           try {
               // 'using' will be used to ensure proper closure of web server after we are done using it
                using(WebClient client = new WebClient()) {
                    int numCommits = 0;
                    client.BaseAddress = URL + USER; // Create a base address
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36"); // Add this on for authentication
                    String json = client.DownloadString(userName + "/repos"); // Enter in the remaining parts of the URL and get the JSON data
                    List<UserInfo> data = JsonConvert.DeserializeObject<List<UserInfo>>(json); // Convert json data into a list
            
                    // Iterate through UserInfo to get the names of all repositories and their commit counts
                    foreach (UserInfo currRepo in data) {
                        String repoName = currRepo.Name;
                        numCommits += Program.Request(userName, repoName, true);
                    }

                    return numCommits;
                }
            } catch(Exception ex) {
                 // Send through exception message to let user know what went wrong
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

    }
}
