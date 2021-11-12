/**
 * FileName: AllCommits.cs
 * Author: Kenneth Lapid David
 *
 * This is a file that contains the AllCommits class which is just a class that will be used to help
 * read json data
 */

using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace dnc200_commits_cli
{
    public class AllCommits
    {
        public List<int> All { get; set; } // Just a field to get the 'all' property from json data

        public List<int> Owner { get; set; } // Just a field to get the 'name' property from json data
    }
}