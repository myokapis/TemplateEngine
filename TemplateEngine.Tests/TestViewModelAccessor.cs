using System;
using System.Collections.Generic;
using NUnit.Framework;
using TemplateEngine;

namespace TemplateEngine.Tests
{
  [TestFixture]
  public class TestViewModelAccessor
  {

    [SetUp]
    public void SetupTest()
    {

    }

    [Test]
    public void TestCount()
    {
      Model1 model1 = new Model1() { PropertyA = "ValueA", PropertyB = "ValueB" };
      Model2 model2 = new Model2() { PropertyA = "ValueA", PropertyB = "ValueB", PropertyC = "ValueC" };

      ViewModelAccessor<Model1> accessor1 = new ViewModelAccessor<Model1>(model1);
      ViewModelAccessor<Model2> accessor2 = new ViewModelAccessor<Model2>(model2);

      Assert.AreEqual(2, accessor1.Count);
      Assert.AreEqual(3, accessor2.Count);
    }

    [Test]
    public void TestCreateObject()
    {
      Model1 model1 = new Model1(){ PropertyA = "ValueA", PropertyB = "ValueB" };
      Model2 model2 = new Model2() { PropertyA = "Value1", PropertyB = "Value2", PropertyC = "Value3" };

      ViewModelAccessor<Model1> accessor1 = new ViewModelAccessor<Model1>(model1);
      ViewModelAccessor<Model2> accessor2 = new ViewModelAccessor<Model2>(model2);

      Assert.IsInstanceOf<ViewModelAccessor<Model1>>(accessor1);
      Assert.IsInstanceOf<ViewModelAccessor<Model2>>(accessor2);
    }

    [Test]
    public void TestEnumerator()
    {
      Model1 model1 = new Model1() { PropertyA = "ValueA", PropertyB = "ValueB" };
      Model2 model2 = new Model2() { PropertyA = "Value1", PropertyB = "Value2", PropertyC = "Value3" };

      ViewModelAccessor<Model1> accessor1 = new ViewModelAccessor<Model1>(model1);
      ViewModelAccessor<Model2> accessor2 = new ViewModelAccessor<Model2>(model2);

      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      Dictionary<string, string> expectedValues1 = new Dictionary<string, string>() { { "PropertyA", model1.PropertyA}, { "PropertyB", model1.PropertyB } };
      Dictionary<string, string> expectedValues2 = new Dictionary<string, string>() { { "PropertyA", model2.PropertyA }, { "PropertyB", model2.PropertyB }, { "PropertyC", model2.PropertyC } };

      foreach (KeyValuePair<string, string> kvp in accessor1.FieldValues)
      {
        dictionary1.Add(kvp.Key, kvp.Value);
      }

      foreach (KeyValuePair<string, string> kvp in accessor2.FieldValues)
      {
        dictionary2.Add(kvp.Key, kvp.Value);
      }

      Assert.AreEqual(expectedValues1, dictionary1);
      Assert.AreEqual(expectedValues2, dictionary2);
    }

    [Test]
    public void TestIndexer_ByIndex()
    {
      Model2 model2 = new Model2() { PropertyA = "Value1", PropertyB = "Value2", PropertyC = "Value3" };

      ViewModelAccessor<Model2> accessor2 = new ViewModelAccessor<Model2>(model2);

      Assert.AreEqual(model2.PropertyA, accessor2[0]);
      Assert.AreEqual(model2.PropertyB, accessor2[1]);
      Assert.AreEqual(model2.PropertyC, accessor2[2]);
    }

    [Test]
    public void TestIndexer_ByFieldName()
    {
      Model2 model2 = new Model2() { PropertyA = "Value1", PropertyB = "Value2", PropertyC = "Value3" };

      ViewModelAccessor<Model2> accessor2 = new ViewModelAccessor<Model2>(model2);

      Assert.AreEqual(model2.PropertyA, accessor2["PropertyA"]);
      Assert.AreEqual(model2.PropertyB, accessor2["PropertyB"]);
      Assert.AreEqual(model2.PropertyC, accessor2["PropertyC"]);
    }

    [Test]
    public void TestModel()
    {
      Model1 model1 = new Model1() { PropertyA = "ValueA", PropertyB = "ValueB" };

      ViewModelAccessor<Model1> accessor1 = new ViewModelAccessor<Model1>(model1);

      Assert.AreEqual(model1.PropertyA, accessor1.Model.PropertyA);
      Assert.AreEqual(model1.PropertyB, accessor1.Model.PropertyB);
      Assert.AreSame(model1, accessor1.Model);
    }

    #region "internal classes"

    public class Model1
    {
      public string PropertyA { get; set; }
      public string PropertyB { get; set; }
    }

    public class Model2
    {
      public string PropertyA { get; set; }
      public string PropertyB { get; set; }
      public string PropertyC { get; set; }
    }

    #endregion
  }
}
