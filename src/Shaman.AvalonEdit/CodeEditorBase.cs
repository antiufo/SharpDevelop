using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shaman.AvalonEdit
{
	abstract class CodeEditorBase : Control
	{
		public Func<Keys, bool> ForwardKeyEvent;

		public abstract Task LoadInstanceAsync(string path = null, string content = null);

		public virtual void RemoveReference(string p)
		{
		}

		public virtual void AddReference(string p)
		{
		}

		public abstract string Code { get; set; }

		public virtual void GoTo(int row, int col)
		{
		}

		public bool AutoSkipUsings { get; set; }

		public abstract void FocusCodeEditor();

		public abstract void Save();


		public abstract string Path { get; }

		public abstract bool IsDirty { get; }

		public Action<bool> OnDirtyChanged { get; set; }
	}
}
