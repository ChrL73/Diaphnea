var mongoose = require("mongoose");
var aSync = require("async");

var db = mongoose.connect("mongodb://localhost/testDb");

var n1 = 100;

var testSchema;
var TestModel;
var testArray;

findDocuments();

function findDocuments()
{
   console.log("n=" + n1 + " findDocuments");
   
   testSchema = mongoose.Schema({ indexField: Number, stringField: String }, { collection: "test" + n1.toString() });
   TestModel = mongoose.model("Test" + n1.toString() , testSchema);
   
   TestModel.find({}, fillArray);
}

function fillArray(err, tests)
{
   //console.log("fillArray");
   
   if (err)
   {
      console.error(err);
   }
   else
   {
      testArray = tests;
      //console.log(testArray);
      setTimeout(function() { testPerf("_id"); }, 1);
   }
}

function testPerf(field)
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
      if (field == "_id")
      {
         TestModel.findOne({ "_id": testArray[j]._id }, function(err, test)
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
      else if (field == "indexField")
      {
         TestModel.findOne({ "indexField": testArray[j].indexField }, function(err, test)
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
         TestModel.findOne({ "stringField": testArray[j].stringField }, function(err, test)
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
         if (field == "_id")
         {
            console.log("dt=" + ((new Date()).getTime() - t0) + "ms (_id)");
            setTimeout(function() { testPerf("indexField"); }, 1);
         }
         else if (field == "indexField")
         {
            console.log("dt=" + ((new Date()).getTime() - t0) + "ms (indexField)");
            setTimeout(function() { testPerf("stringField"); }, 1);
         }
         else
         {
            console.log("dt=" + ((new Date()).getTime() - t0) + "ms (stringField)");
            if (n1 < 30000)
            {
               n1 *= 2;
               setTimeout(findDocuments(), 1);
            }
            else     
            {
               console.log("End of test");
            }
         }
         
      }
   });
}
