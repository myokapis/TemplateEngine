using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: determine if this class is even necessary
namespace TemplateEngine
{
  public abstract class PresenterBase
  {
    // TODO: implement the presenters then refactor common/expected methods into this base class using abstract methods where it makes sense to do so
    public abstract string GetContent();
  }
}
