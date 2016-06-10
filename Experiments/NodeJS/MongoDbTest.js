// This script permits to see that:
//  - When the search key is not an index: The execution time of the 'findOne' method grows linearly with the size of the collection.
//  - When the search key is '_id': The execution time of the 'findOne' method stays constant when the collection size grows.

// (See graphics in MongDbTest.ods)

var mongoose = require("mongoose");
var aSync = require("async");

var db = mongoose.connect("mongodb://localhost/testDb");
var testSchema = mongoose.Schema({ stringField: String }, { collection: "test" });
var TestModel = mongoose.model("Test", testSchema);

var testArray1 = [];
var iArray = [];
var n1 = 100;
launchTest();

function launchTest()
{
   console.log("launchTest, n=" + n1); 
   var i;
   for (i = 0; i < n1; ++i)
   {
      iArray.push(i);
      testArray1.push(getString(i + 1));
   }
   
   //console.log(testArray1);
   //console.log(iArray);
   
   TestModel.remove({}, addDocuments);
}

function addDocuments(err)
{
   //console.log("addDocuments");
   if (err)
   {
      console.log(err);
   }
   else
   {
      aSync.each(iArray, function(j, callback)
      {
         TestModel.create({ "stringField": testArray1[j] }, function()
         {
            callback(false);
         });
      },
      function(err)
      {
         if (err) console.log("err = " + err);
         else findDocuments();
      });
   }
}

function findDocuments()
{
   //console.log("findDocuments");
   TestModel.find({}, fillArray);
}

var testArray2;

function fillArray(err, tests)
{
   //console.log("fillArray");
   
   if (err)
   {
      console.error(err);
   }
   else
   {
      testArray2 = tests;
      //console.log(testArray2);
      setTimeout(function() { testPerf(true); }, 1);
   }
}

function testPerf(id)
{
   //console.log("testPerf");
   
   var j, n2 = 1000;
   var indexArray = [];
   for (j = 0; j < n2; ++j)
   {
      indexArray.push(Math.floor(Math.random() * n1));
   }
   
   //console.log(indexArray);
   
   var t0 = (new Date()).getTime();
   
   aSync.each(indexArray, function(j, callback)
   {
      if (id)
      {
         TestModel.findOne({ "_id": testArray2[j]._id }, function(err, test)
         {
            if (err || !test)
            {
               console.log("err: " + err + " test: " + test);
               callback(true);
            }
            else
            {
               callback(false);
            }
         });
      }
      else
      {
         TestModel.findOne({ "stringField": testArray2[j].stringField }, function(err, test)
         {
            if (err || !test)
            {
               console.log("err: " + err + " test: " + test);
               callback(true);
            }
            else
            {
               callback(false);
            }
         });
      }
   },
   function(err)
   {
      if (err)
      {
         console.log("err = " + err);
      }
      else
      {
         if (id)
         {
            console.log("dt=" + ((new Date()).getTime() - t0) + "ms (_id)");
            setTimeout(function() { testPerf(false); }, 1);
         }
         else 
         {
            console.log("dt=" + ((new Date()).getTime() - t0) + "ms (stringField)");
            if (n1 < 30000)
            {
               n1 *= 2;
               setTimeout(launchTest(), 1);
            }
            else     
            {
               console.log("End of test");
            }
         }
         
      }
   });
}

function getString(i)
{
   var s = "";
   var value = i;
   var j;
   for (j = 0; j < 10; ++j)
   {
      value = Math.exp(value) % 700;  
      s = s + String.fromCharCode((value % 26) + 97);
   }
   
   s = s + i.toString(); 
   return s;
}
