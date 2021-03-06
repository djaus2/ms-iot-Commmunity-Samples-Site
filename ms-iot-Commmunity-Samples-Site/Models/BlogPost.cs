﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using msiotCommunitySamples.Utilities;
using System.Reflection;
using Nancy;
using Newtonsoft.Json;

namespace msiotCommunitySamples.Models
{
    [Serializable]
    public class BlogPost
    {
        private static int Count = 0;
        public int Id { get; set; }

        //<a href="/Show/1">@objUser.Title</a>
        public string TitleId
        {
            get
            {
                string ret = "/ms_iot_Community_Samples/Show/" + Id;
                return ret;
            }
        }
        public string filenameDisp
        {
            get
            {
                string ret = "/ms_iot_Community_Samples/display/" + filename;
                return ret;
            }
        }
        

        //public string Title { get; set; }
        //public string Summary { get; set; }

        private const int SummarySubLength = 50;

        public string SummarySub
        {
            get
            {

                if (description.Length > SummarySubLength)
                    return description.Substring(0, SummarySubLength) + " ...";
                else
                    return description;

            }
        }

        public string layout { get; set; } = "";
        public string filename { get; set; } = "";
        public string title { get; set; } = "";
        public string description { get; set; } = "";
        public string keyword { get; set; } = "";
        public string permalink { get; set; } = "";
        public string samplelink { get; set; } = "";
        public string lang { get; set; } = "en-us";

        //public string Description { get; set; }

        //public List<string> Authors { get; set; }

        //public List<string> CodeLanguages { get; set; }

        //public string Language { get; set; } = "en-us";

        //public string GitHubRepository_URL { get; set; }

        //public string HacksterIO_URL { get; set; } = "";

        //public List<string> Tags { get; set; }

        public static List<string> Fields;

        public BlogPost()
        {
            //Tags = new List<string>();
            //Authors = new List<string>();
            //CodeLanguages = new List<string>();
            if (BlogPostz == null)
            {
                ClearBlogPostz();
    //            Count = 0;
    //            var fields = typeof(BlogPost).GetFields(
    //BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                //            var names = Array.ConvertAll(fields, field => field.Name.Substring(1).Replace(">k__BackingField",""));
                //            Fields = names.ToList<string>();
                //            //Type myType = typeof(BlogPost);
                //            //var myField = myType.GetFields();
                //            //Fields = new List<string>();
                //            //for (int i = 0; i < myField.Length; i++)
                //            //{
                //            //    Fields.Add(myField[i].Name);
                //            //}
                //            BlogPostz = new List<BlogPost>();
            }
            Id = Count++;
            BlogPostz.Add(this);
        }

        //This is the underlying collection of BlogPosts that gets filtered and sorted.
        private static List<BlogPost> _BlogPostz = null;
        public static List<BlogPost> BlogPostz {
            get { return _BlogPostz;  }
            set
            {
                _BlogPostz = value;
            }
        }

  
        /// <summary>
        /// Use this in views
        /// </summary>
        public static List<BlogPost> ViewBlogPostz (string filtersStr)
        {
            var blogs = from n in BlogPostz select n;
            List<BlogPost> blogg = blogs.ToList<BlogPost>();
            if (filtersStr != "")
            {
                JsonSerializerSettings set = new JsonSerializerSettings();
                set.MissingMemberHandling = MissingMemberHandling.Ignore;
                var filter = JsonConvert.DeserializeObject<FilterAndSortInfo>(filtersStr, set);
                if (filter.Filters != null)
                {
                    blogg = Models.BlogPost.Filter(filter.Filters);
                }
                if (filter.SortString != "")
                {
                    blogg = Sort(blogg, filter.SortString, filter.LastSort, filter.LastSortDirection);
                }
                
            }
            return blogg;
        }

        ////Remove filtering and sotrting from views
        //public static void ResetBlogPostz()
        //{
        //    ////ViewBlogPostz = (from n in BlogPostz select n).ToList<BlogPost>();
        //    //LastSort = "";
        //    //LastSortDirection = "desc";
        //}

        internal static BlogPost Get(string id)
        {
            var bp = from n in BlogPostz where n.Id.ToString() == id select n;
            if (bp.Count() != 0)
                return bp.First();
            else
                return null;
        }


        //Clear rhe collection and therefore the view
        public static void ClearBlogPostz()
        {
            Count = 0;
            var fields = typeof(BlogPost).GetFields(
BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var names = Array.ConvertAll(fields, field => field.Name.Substring(1).Replace(">k__BackingField", ""));

            Fields = names.ToList<string>();
            BlogPostz = new List<BlogPost>();
        }


        //Sort the view on one field
        public static List<BlogPost> Sort(List<BlogPost> viewBlogPostz, string SortString, string LastSort, string LastSortDirection)
        {

            if (string.IsNullOrWhiteSpace(SortString))
            {
                // If no sorting string, give a message and return.
                System.Diagnostics.Debug.WriteLine("Please type in a sorting string.");
                return viewBlogPostz;
            }

            try
            {
                // Prepare the sorting string into a list of Tuples
                var sortExpressions = new List<Tuple<string, string>>();
                string[] terms = SortString.Split(',');
                for (int i = 0; i < terms.Length; i++)
                {
                    string[] items = terms[i].Trim().Split('~');
                    var fieldName = items[0].Trim();
                    var sortOrder = (items.Length > 1)
                              ? items[1].Trim().ToLower() : "";
                    if ((sortOrder != "asc") && (sortOrder != "desc"))
                    {
                        //throw new ArgumentException("Invalid sorting order");
                        if (LastSort == fieldName)
                        {
                            if (LastSortDirection == "desc")
                                sortOrder = "asc";
                            else
                                sortOrder = "desc";
                        }
                        else
                            sortOrder = "asc";

                    }
                    LastSort = fieldName;
                    LastSortDirection = sortOrder;
                    sortExpressions.Add(new Tuple<string, string>(fieldName, sortOrder));
                }

                // Apply the sorting
                viewBlogPostz = viewBlogPostz.MultipleSort(sortExpressions).ToList();
                return viewBlogPostz;
            }
            catch (Exception e)
            {
                var msg = "There is an error in your sorting string.  Please correct it and try again - "
              + e.Message;
                System.Diagnostics.Debug.WriteLine(msg);
                return BlogPostz;
            }
        }

        //Filter the view on one field
        public static List<BlogPost> Filter(List<Tuple<string, string>> filters)
        {
            var lst = (from n in BlogPostz select n).ToList<BlogPost>();

            for (int index = 0; index < filters.Count; index++)
            {
                lst = FilterGen<BlogPost>(lst, filters[index].Item1, filters[index].Item2);
            }
            return lst.ToList<BlogPost>();
        }

        public static List<T> FilterGen<T>(
            List<T> collection,
            string property,
            string filterValue)
        {
            var filteredCollection = new List<T>();
            foreach (T item in collection)
            {
                // To check multiple properties use,
                // item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)

                var propertyInfo =
                    item.GetType()
                        .GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                    throw new NotSupportedException("property given does not exists");
                if (    //String use contatins
                        (propertyInfo.PropertyType.Name == "string")
                        ||
                        (propertyInfo.PropertyType.Name == "String")
                    )
                {
                    var propertyValue = (string)propertyInfo.GetValue(item, null);
                    if (propertyValue.ToString().Contains(filterValue))
                        filteredCollection.Add(item);
                }
                else if ( //Id use exact
                            (propertyInfo.PropertyType.Name == "Int64")
                            ||
                            (propertyInfo.PropertyType.Name == "Int32")
                        )
                {
                    int propertyValue2 = (int)propertyInfo.GetValue(item, null);
                    if (propertyValue2.ToString()== filterValue)
                        filteredCollection.Add(item);
                }
                else if (propertyInfo.PropertyType.Name == "List`1")
                {
                    if (property=="CodeLanguages")
                    {
                        var propertyValue3 = (List<string>)propertyInfo.GetValue(item, null);
                        if (propertyValue3.Contains(filterValue))
                            filteredCollection.Add(item);
                    }
                    if (property == "Tags")
                    {
                        var propertyValue4 = (List<string>)propertyInfo.GetValue(item, null);
                        if (propertyValue4.Contains(filterValue))
                            filteredCollection.Add(item);
                    }

                }
            }

            return filteredCollection;
        }

        public static List<T> FilterXX<T>(List<T> list, Func<T, bool> filter)
        {
            var temp = list.AsQueryable().Where(filter);
            return temp.Cast<T>().ToList();
        }
    }


}


