// Copyright(c) 2017 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using CommandLine;
using Google.Cloud.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleCloudSamples
{
    public class AddData
    {
        public static string Usage = @"Usage:
C:\> dotnet run command YOUR_PROJECT_ID

Where command is one of
    add-doc-as-map
    update-create-if-missing
    add-doc-data-types
    add-simple-doc-as-entity
    set-requires-id
    add-doc-data-with-auto-id
    add-doc-data-after-auto-id
    update-doc
    update-nested-fields
    update-server-timestamp
    update-document-array
";
        private static async Task AddDocAsMap(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_add_doc_as_map]
            DocumentReference docRef = db.Collection("cities").Document("LA");
            Dictionary<string, object> city = new Dictionary<string, object>
            {
                { "name", "Los Angeles" },
                { "state", "CA" },
                { "country", "USA" }
            };
            await docRef.SetAsync(city);
            // [END fs_add_doc_as_map]
            Console.WriteLine("Added data to the LA document in the cities collection.");
        }

        private static async Task UpdateCreateIfMissing(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_update_create_if_missing]
            DocumentReference docRef = db.Collection("cities").Document("LA");
            Dictionary<string, object> update = new Dictionary<string, object>
            {
                { "capital", false }
            };
            await docRef.SetAsync(update, SetOptions.MergeAll);
            // [END fs_update_create_if_missing]
            Console.WriteLine("Merged data into the LA document in the cities collection.");
        }

        private static async Task AddDocDataTypes(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_add_doc_data_types]
            DocumentReference docRef = db.Collection("data").Document("one");
            Dictionary<string, object> docData = new Dictionary<string, object>
            {
                { "stringExample", "Hello World" },
                { "booleanExample", false },
                { "numberExample", 3.14159265 },
                { "nullExample", null },
            };

            ArrayList arrayExample = new ArrayList();
            arrayExample.Add(5);
            arrayExample.Add(true);
            arrayExample.Add("Hello");
            docData.Add("arrayExample", arrayExample);

            Dictionary<string, object> objectExample = new Dictionary<string, object>
            {
                { "a", 5 },
                { "b", true },
            };
            docData.Add("objectExample", objectExample);

            await docRef.SetAsync(docData);
            // [END fs_add_doc_data_types]
            Console.WriteLine("Set multiple data-type data for the one document in the data collection.");
        }

        // [START fs_class_definition]
        [FirestoreData]
        public class City
        {
            [FirestoreProperty]
            public string Name { get; set; }

            [FirestoreProperty]
            public string State { get; set; }

            [FirestoreProperty]
            public string Country { get; set; }

            [FirestoreProperty]
            public bool Capital { get; set; }

            [FirestoreProperty]
            public long Population { get; set; }
        }
        // [END fs_class_definition]

        private static async Task AddSimpleDocAsEntity(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_add_simple_doc_as_entity]
            DocumentReference docRef = db.Collection("cities").Document("LA");
            City city = new City
            {
                Name = "Los Angeles",
                State = "CA",
                Country = "USA",
                Capital = false,
                Population = 3900000L
            };
            await docRef.SetAsync(city);
            // [END fs_add_simple_doc_as_entity]
            Console.WriteLine("Added custom City object to the cities collection.");
        }

        private static async Task SetRequiresId(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            Dictionary<string, object> city = new Dictionary<string, object>
            {
                { "Name", "Phuket" },
                { "Country", "Thailand" }
            };
            // [START fs_set_requires_id]
            await db.Collection("cities").Document("new-city-id").SetAsync(city);
            // [END fs_set_requires_id]
            Console.WriteLine("Added document with ID: new-city-id.");
        }

        private static async Task AddDocDataWithAutoId(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_add_doc_data_with_auto_id]
            Dictionary<string, object> city = new Dictionary<string, object>
            {
                { "Name", "Tokyo" },
                { "Country", "Japan" }
            };
            DocumentReference addedDocRef = await db.Collection("cities").AddAsync(city);
            Console.WriteLine("Added document with ID: {0}.", addedDocRef.Id);
            // [END fs_add_doc_data_with_auto_id]
        }

        private static async Task AddDocDataAfterAutoId(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            Dictionary<string, object> city = new Dictionary<string, object>
            {
                { "Name", "Moscow" },
                { "Country", "Russia" }
            };
            // [START fs_add_doc_data_after_auto_id]
            DocumentReference addedDocRef = db.Collection("cities").Document();
            Console.WriteLine("Added document with ID: {0}.", addedDocRef.Id);
            await addedDocRef.SetAsync(city);
            // [END fs_add_doc_data_after_auto_id]
            Console.WriteLine("Added data to the {0} document in the cities collection.", addedDocRef.Id);
        }

        private static async Task UpdateDoc(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_update_doc]
            DocumentReference cityRef = db.Collection("cities").Document("new-city-id");
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Capital", false }
            };
            await cityRef.UpdateAsync(updates);

            // You can also update a single field with: await cityRef.UpdateAsync("Capital", false);
            // [END fs_update_doc]
            Console.WriteLine("Updated the Capital field of the new-city-id document in the cities collection.");
        }

        private static async Task UpdateNestedFields(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_update_nested_fields]
            DocumentReference frankDocRef = db.Collection("users").Document("frank");
            Dictionary<string, object> initialData = new Dictionary<string, object>
            {
                { "Name", "Frank" },
                { "Age", 12 }
            };

            Dictionary<string, object> favorites = new Dictionary<string, object>
            {
                { "Food", "Pizza" },
                { "Color", "Blue" },
                { "Subject", "Recess" },
            };
            initialData.Add("Favorites", favorites);
            await frankDocRef.SetAsync(initialData);

            // Update age and favorite color
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "Age", 13 },
                { "Favorites.Color", "Red" },
            };

            // Asynchronously update the document
            await frankDocRef.UpdateAsync(updates);
            // [END fs_update_nested_fields]
            Console.WriteLine("Updated the age and favorite color fields of the Frank document in the users collection.");
        }

        private static async Task UpdateServerTimestamp(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_update_server_timestamp]
            DocumentReference cityRef = db.Collection("cities").Document("new-city-id");
            await cityRef.UpdateAsync("Timestamp", Timestamp.GetCurrentTimestamp());
            // [END fs_update_server_timestamp]
            Console.WriteLine("Updated the Timestamp field of the new-city-id document in the cities collection.");
        }

        private static async Task UpdateDocumentArray(string project)
        {
            FirestoreDb db = FirestoreDb.Create(project);
            // [START fs_update_document_array]
            DocumentReference washingtonRef = db.Collection("cities").Document("DC");

            // Atomically add a new region to the "regions" array field.
            await washingtonRef.UpdateAsync("Regions", FieldValue.ArrayUnion("greater_virginia"));

            // Atomically remove a region from the "regions" array field.
            await washingtonRef.UpdateAsync("Regions", FieldValue.ArrayRemove("east_coast"));
            // [END fs_update_document_array]
            Console.WriteLine("Updated the Regions array of the DC document in the cities collection.");
        }

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Write(Usage);
                return;
            }
            string command = args[0].ToLower();
            string project = string.Join(" ",
                new ArraySegment<string>(args, 1, args.Length - 1));
            switch (command)
            {
                case "add-doc-as-map":
                    AddDocAsMap(project).Wait();
                    break;

                case "update-create-if-missing":
                    UpdateCreateIfMissing(project).Wait();
                    break;

                case "add-doc-data-types":
                    AddDocDataTypes(project).Wait();
                    break;

                case "add-simple-doc-as-entity":
                    AddSimpleDocAsEntity(project).Wait();
                    break;

                case "set-requires-id":
                    SetRequiresId(project).Wait();
                    break;

                case "add-doc-data-with-auto-id":
                    AddDocDataWithAutoId(project).Wait();
                    break;

                case "add-doc-data-after-auto-id":
                    AddDocDataAfterAutoId(project).Wait();
                    break;

                case "update-doc":
                    UpdateDoc(project).Wait();
                    break;

                case "update-nested-fields":
                    UpdateNestedFields(project).Wait();
                    break;

                case "update-server-timestamp":
                    UpdateServerTimestamp(project).Wait();
                    break;

                case "update-document-array":
                    UpdateDocumentArray(project).Wait();
                    break;

                default:
                    Console.Write(Usage);
                    return;
            }
        }
    }
}
