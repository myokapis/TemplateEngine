using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using TemplateEngine;

namespace TemplateEngine.Tests
{
  [TestFixture]
  public class TestTemplateExtensions
  {

    [Test]
    public void TestCreateObject_NoParams()
    {
      TemplateEngine.Template tpl = new TemplateEngine.Template();
      Assert.IsInstanceOf<TemplateEngine.Template>(tpl);
    }

    [Test]
    public void TestCreateObject_FromFile()
    {
      TemplateEngine.Template tpl = new TemplateEngine.Template(CreateTemplateFile(""));
      Assert.IsInstanceOf<TemplateEngine.Template>(tpl);
    }

    [Test]
    public void TestSetField()
    {
      string[] data = TemplateData();
      TemplateEngine.Template tpl = new TemplateEngine.Template(CreateTemplateFile(String.Join("", data)));
      List<DataHelper1> fieldData = new List<DataHelper1>()
      {
        new DataHelper1(){Field1 = "One", Field2 = "Two"},
        new DataHelper1(){Field1 = "1", Field2 = "2"},
        new DataHelper1(){Field1 = "Ein", Field2 = "Zwei"}
      };

      StringBuilder sb = new StringBuilder();
      foreach (DataHelper1 item in fieldData)
      {
        sb.Append(data[3].Replace("@@Field1@@", item.Field1).Replace("@@Field2@@", item.Field2));
      }

      tpl.selectSection("MAIN");
      tpl.selectSection("LEVEL_ONE");
      tpl.selectSection("LEVEL_TWO");
      tpl.setField("Field1", fieldData[0].Field1);
      tpl.setField("Field2", fieldData[0].Field2);
      tpl.appendSection();
      tpl.setField("Field1", fieldData[1].Field1);
      tpl.setField("Field2", fieldData[1].Field2);
      tpl.appendSection();
      tpl.setField("Field1", fieldData[2].Field1);
      tpl.setField("Field2", fieldData[2].Field2);
      tpl.appendSection();
      tpl.deselectSection();
      tpl.appendSection();
      tpl.deselectSection();
      tpl.appendSection();
      tpl.deselectSection();
      string output = tpl.getContent();

      Assert.AreEqual(sb.ToString(), output);
    }

    [Test]
    public void TestSetSectionFields()
    {
      string[] data = TemplateData();
      TemplateEngine.Template tpl = new TemplateEngine.Template(CreateTemplateFile(String.Join("", data)));
      List<DataHelper1> fieldData = new List<DataHelper1>()
      {
        new DataHelper1(){Field1 = "One", Field2 = "Two"},
        new DataHelper1(){Field1 = "1", Field2 = "2"},
        new DataHelper1(){Field1 = "Ein", Field2 = "Zwei"}
      };

      StringBuilder sb = new StringBuilder();
      foreach (DataHelper1 item in fieldData)
      {
        sb.Append(data[3].Replace("@@Field1@@", item.Field1).Replace("@@Field2@@", item.Field2));
      }

      tpl.selectSection("MAIN");
      tpl.selectSection("LEVEL_ONE");
      tpl.selectSection("LEVEL_TWO");
      tpl.setSectionFields(fieldData);
      tpl.deselectSection();
      tpl.appendSection();
      tpl.deselectSection();
      tpl.appendSection();
      tpl.deselectSection();
      string output = tpl.getContent();

      Assert.AreEqual(sb.ToString(), output);
    }

    #region "Helper Methods"

    private string CreateTemplateFile(string Contents)
    {
      string filePath = Path.GetTempFileName();

      using (TextWriter writer = new StreamWriter(filePath))
      {
        writer.Write(Contents);
      }

      return filePath;
    }

    private string[] TemplateData()
    {
      string[] data = {
        "<!-- @@MAIN@@ -->",
        "<!-- @@LEVEL_ONE@@ -->",
        "<!-- @@LEVEL_TWO@@ -->",
        "<div><span>@@Field1@@</span><span>@@Field2@@</span></div>\r\n",
        "<!-- @@LEVEL_TWO@@ -->",
        "<!-- @@LEVEL_ONE@@ -->",
        "<!-- @@MAIN@@ -->"
      };

      return data;
    }

    #endregion

    #region "internal classes"

    private class DataHelper1
    {
      public string Field1 { get; set; }
      public string Field2 { get; set; }
    }

    #endregion
  }
}
