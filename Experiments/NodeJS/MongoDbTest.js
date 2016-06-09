var mongoose = require("mongoose");
var aSync = require("async");

var n = 10000;
var testArray1 = [];
var iArray = [];
var i;
for (i = 0; i < n; ++i)
{
   iArray.push(i);
   testArray1.push(getString(i + 1));
}

//console.log(testArray1);
//console.log(iArray);

var db = mongoose.connect("mongodb://localhost/testDb");

var testSchema = mongoose.Schema({ stringField: String }, { collection: "test" });
var TestModel = mongoose.model("Test", testSchema);

TestModel.remove({}, addDocuments);

function addDocuments(err)
{
   console.log("addDocuments");
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
   console.log("findDocuments");
   TestModel.find({}, fillArray);
}

var testArray2;

function fillArray(err, tests)
{
   console.log("fillArray");
   
   if (err)
   {
      console.error(err);
   }
   else
   {
      testArray2 = tests;
      //console.log(testArray2);
      setTimeout(testPerf, 1);
   }
}

function testPerf()
{
   console.log("testPerf");
   
   var j, n2 = 1000;
   var indexArray = [];
   for (j = 0; j < n2; ++j)
   {
      indexArray.push(Math.floor(Math.random() * n));
   }
   
   //console.log(indexArray);
   
   var t0 = (new Date()).getTime();
   
   aSync.each(indexArray, function(j, callback)
   {
      TestModel.findOne({ "stringField": testArray2[j].stringField }, function(err, test)
      //TestModel.findOne({ "_id": testArray2[j]._id }, function(err, test)
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
   },
   function(err)
   {
      if (err)
      {
         console.log("err = " + err);
      }
      else
      {
         console.log("dt=" + ((new Date()).getTime() - t0) + "ms");
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
