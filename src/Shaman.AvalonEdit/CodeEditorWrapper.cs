#if !NO_ICSHARPCODE
using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Shaman.Runtime;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Parser;

namespace Shaman.AvalonEdit
{
	class CodeEditorWrapper : CodeEditorBase
	{
		private System.Windows.Forms.Integration.ElementHost host;

		private static AvalonEditDisplayBinding avalonEditDisplayBinding;
		private Label loadingLabel;
		private static System.Threading.Tasks.Task staticLoadedTask;
		private CodeEditor codeEditor;

		public CodeEditorWrapper()
		{
			if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
				this.Controls.Add(new Label() { Text = "Code editor" });
			this.Disposed += CodeEditorWrapper_Disposed;
		}



		private OpenedFile openedFile;
		private string _path;
		public override string Path { get { return _path; } }
		private AvalonEditViewContent content;
		private string _codeToLoad;

		public override async System.Threading.Tasks.Task LoadInstanceAsync(string path = null, string content = null)
		{
			this._path = path;
			_codeToLoad = content;
			if (staticLoadedTask == null) staticLoadedTask = LoadStaticAsync();
			if (!staticLoadedTask.IsCompleted && !IsPrefetchingInstance)
			{
				this.loadingLabel = new Label()
				{
					Text = "Loading editor...",
					Dock = DockStyle.Fill,
					TextAlign = System.Drawing.ContentAlignment.MiddleCenter
				};
				this.Controls.Add(loadingLabel);
				await staticLoadedTask;
				if (IsDisposed || Disposing) return;
				this.Controls.Remove(loadingLabel);
				loadingLabel.Dispose();
			}
			CompleteLoadInstance(path, content);

		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void CompleteLoadInstance(string path, string code)
		{


			openedFile = path != null ? ICSharpCode.SharpDevelop.FileService.OpenFile(path).PrimaryFile : SD.FileService.CreateUntitledOpenedFile("code.cs", Encoding.UTF8.GetBytes(code));



			openedFile.IsDirtyChanged += openedFile_IsDirtyChanged;

			content = (AvalonEditViewContent)openedFile.RegisteredViewContents.SingleOrDefault();
			if (content == null)
				content = (AvalonEditViewContent)avalonEditDisplayBinding.CreateContentForFile(openedFile);

			codeEditor = (CodeEditor) content.Control;

			host = new ElementHost();
			//codeEditor.Children.Clear();
			//codeEditor.Children.Add(new System.Windows.Controls.Button());
			codeEditor.KeyDown += codeEditor_KeyDown;
			host.Child = codeEditor;



			AddReference(typeof(System.Uri).Assembly.Location);
			AddReference(typeof(System.Linq.Enumerable).Assembly.Location);


			if (AutoSkipUsings)
			{
				var p = Code.IndexOf('{');
				if (p != -1)
				{
					var line = codeEditor.PrimaryTextEditor.Document.GetLineByOffset(p).LineNumber;
					codeEditor.PrimaryTextEditor.CaretOffset = p;
					codeEditor.PrimaryTextEditor.ScrollToLine(line);
				}
			}


			//host.Child = codeEditor;
			//1host.Child = new ICSharpCode.AvalonEdit.TextEditor()
			//host.Child = new System.Windows.Controls.Button();
			host.Dock = DockStyle.Fill;
			Controls.Add(host);
			host.BringToFront();
		}

		void codeEditor_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			var ctrl = (Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Control) != System.Windows.Input.ModifierKeys.None;
			var shift = (Keyboard.Modifiers & System.Windows.Input.ModifierKeys.Shift) != System.Windows.Input.ModifierKeys.None;
			var key = (Keys)KeyInterop.VirtualKeyFromKey(e.Key);
			if (ctrl) key |= Keys.Control;
			if (shift) key |= Keys.Shift;
			if (ForwardKeyEvent != null && ForwardKeyEvent(key))
				e.Handled = true;



		}


		private static AwdeeCodeWorkbench Workbench;



		void openedFile_IsDirtyChanged(object sender, EventArgs e)
		{
			_isDirty = openedFile.IsDirty;
			if (OnDirtyChanged != null) OnDirtyChanged(openedFile.IsDirty);
		}

		public override void GoTo(int line, int column)
		{
			if (codeEditor == null) return;
			var offset = codeEditor.PrimaryTextEditor.Document.GetOffset(line, column);
			//codeEditor.PrimaryTextEditor.Select(0, );
			codeEditor.PrimaryTextEditor.CaretOffset = offset;
			codeEditor.PrimaryTextEditor.ScrollToLine(line);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void PreloadAsync()
		{
			if (staticLoadedTask == null) staticLoadedTask = LoadStaticAsync();
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (codeEditor != null)
				{
					openedFile.UnregisterView(content);
					openedFile.CloseIfAllViewsClosed();
					//                 if (openedFile.RegisteredViewContents.Count == 0)
					Workbench.ViewContentCollection.Remove(content);
					codeEditor.Dispose();
					codeEditor = null;
				}
			}
			base.Dispose(disposing);
		}


		[MethodImpl(MethodImplOptions.NoInlining)]
		private static async System.Threading.Tasks.Task LoadStaticAsync()
		{
			var mutableSyncCtx = new SynchronizingObjectAdapter();
			var mainThreadId = Environment.CurrentManagedThreadId;
			var mainSyncCtx = SynchronizationContext.Current;
			var completed = new TaskCompletionSource<bool>();
			
			//  var thread = new Thread(() =>
			{
				try
				{
					PreloadInternal(mutableSyncCtx, mainThreadId, mainSyncCtx, completed);
				}
				catch (Exception ex)
				{
					completed.SetException(ex);
				}
			}//);
			 /*
			 thread.IsBackground = true;
			 thread.SetApartmentState(ApartmentState.STA);
			 await System.Threading.Tasks.Task.Yield();
			 thread.Start();
			 */
			await completed.Task;
		}




		private static void AddAddInResource(CoreStartup startup, string s)
		{
			var tree = SD.AddInTree;
			using (var sr= new StringReader(s))
			{
				var addin = AddIn.Load(tree, sr);
				addin.Enabled = true;
			}
			/*
			using (StringReader stringReader = new StringReader(s))
			{
				AddIn addIn = AddIn.Load(stringReader, null, null);
				addIn.Enabled = true;
				AddInTree.InsertAddIn(addIn);
			}
			*/
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void PreloadInternal(SynchronizingObjectAdapter mutableSyncCtx, int mainThreadId, SynchronizationContext mainSyncCtx, TaskCompletionSource<bool> completed)
		{
			//   ICSharpCode.Core.Runtime.IsBundledBuild = true;
			var tempSyncCtx = new WindowsFormsSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(tempSyncCtx);
			mutableSyncCtx.ThreadId = Environment.CurrentManagedThreadId;
			mutableSyncCtx.SyncCtx = tempSyncCtx;
			var container = new System.ComponentModel.Design.ServiceContainer(ServiceSingleton.FallbackServiceProvider);
			container.AddService(typeof(IFileService), typeof(ICSharpCode.SharpDevelop.Workbench.FileService));
			container.AddService(typeof(IParserService), typeof(ParserService));
			
			//container.AddService(typeof(IMessageService), new SDMessageService());
			//container.AddService(typeof(ILoggingService), new log4netLoggingService());
			ServiceSingleton.ServiceProvider = container;




			var startup = new CoreStartup("Shaman.AvalonEdit");
			var propertyService = new ICSharpCode.SharpDevelop.PropertyService();
			startup.StartCoreServices(propertyService);

			AddAddInResource(startup, GetText(Properties.Resources.ICSharpCode_SharpDevelop));
			AddAddInResource(startup, GetText(Properties.Resources.CSharpBinding));
			AddAddInResource(startup, GetText(Properties.Resources.AvalonEdit_AddIn));


			startup.RunInitialization();

			var wb = new AwdeeCodeWorkbench(mutableSyncCtx);
			Workbench = wb;
			//var wbl = new AwdeeCodeWorkbenchLayout();
			//WorkbenchSingleton.InitializeWorkbench(wb, wbl);
			var opt = CodeEditorOptions.Instance;
			opt.EnableQuickClassBrowser = false;
			opt.AllowScrollBelowDocument = true;
			opt.AutoInsertBlockEnd = false;
			opt.ConvertTabsToSpaces = true;
			opt.EnableAnimations = true;
			opt.EnableEmailHyperlinks = false;
			opt.EnableRectangularSelection = true;
			opt.EnableTextDragDrop = true;
			opt.EnableVirtualSpace = false;
			opt.IndentationSize = 4;
			opt.ShowLineNumbers = false;
			opt.UnderlineErrors = true;
			opt.ShowColumnRuler = false;
			opt.EnableHyperlinks = false;
			avalonEditDisplayBinding = new AvalonEditDisplayBinding();


			//var preload = new CodeEditorWrapper();
			//preload.IsPrefetchingInstance = true;

			//SingleThreadSynchronizationContext.Run(async () =>
			//{
			//	SynchronizationContext.SetSynchronizationContext(tempSyncCtx);
			//	await preload.LoadInstanceAsync(content: @"using System; namespace DummyPrefetching {}");
			//});

			//preload.Dispose();
			mutableSyncCtx.ThreadId = mainThreadId;
			mutableSyncCtx.SyncCtx = mainSyncCtx;
			completed.SetResult(true);
		}

		private static string GetText(byte[] utf8)
		{
			using (var m = new MemoryStream(utf8))
			using (var sr = new StreamReader(m, Encoding.UTF8, true))
			{
				return sr.ReadToEnd();
			}
		}

		class AwdeeCodeStatusBar : IStatusBarService
		{
			public void SetCaretPosition(int x, int y, int charOffset)
			{
			}

			public void SetMessage(string message, bool highlighted = false, ICSharpCode.SharpDevelop.IImage icon = null)
			{
			}

			public IProgressMonitor CreateProgressMonitor(System.Threading.CancellationToken cancellationToken)
			{
				throw new NotImplementedException();
			}

			public void AddProgress(ProgressCollector progress)
			{
			}

			public void SetSelectionSingle(int length)
			{
			}

			public void SetSelectionMulti(int rows, int cols)
			{
			}
		}



		class AwdeeCodeWorkbench : IWorkbench
		{
			public AwdeeCodeWorkbench(SynchronizingObjectAdapter sync)
			{
				this._synchronizingObject = sync;
				_mainWindow = new System.Windows.Window();
			}

			event EventHandler<ViewContentEventArgs> IWorkbench.ViewOpened
			{
				add
				{
					throw new NotImplementedException();
				}

				remove
				{
					throw new NotImplementedException();
				}
			}

			event EventHandler<ViewContentEventArgs> IWorkbench.ViewClosed
			{
				add
				{
					throw new NotImplementedException();
				}

				remove
				{
					throw new NotImplementedException();
				}
			}

			public System.Windows.Forms.IWin32Window MainWin32Window
			{
				get { throw new NotImplementedException(); }
			}

			public System.ComponentModel.ISynchronizeInvoke SynchronizingObject
			{
				get { return _synchronizingObject; }
			}

			public System.Windows.Window MainWindow
			{
				get { return _mainWindow; }
			}

			public IStatusBarService StatusBar
			{
				get
				{
					return _statusBar;
				}
			}

			public bool FullScreen
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public string Title
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public ICollection<IViewContent> ViewContentCollection
			{
				get { return _viewContentCollection; }
			}

			public ICollection<IViewContent> PrimaryViewContents
			{
				get { throw new NotImplementedException(); }
			}

			public IList<IWorkbenchWindow> WorkbenchWindowCollection
			{
				get { throw new NotImplementedException(); }
			}

			public IList<ICSharpCode.SharpDevelop.PadDescriptor> PadContentCollection
			{
				get { throw new NotImplementedException(); }
			}

			public IWorkbenchWindow ActiveWorkbenchWindow
			{
				get { throw new NotImplementedException(); }
			}

#pragma warning disable 0067
			public event EventHandler ActiveWorkbenchWindowChanged;
			public event EventHandler ActiveViewContentChanged;
			public event EventHandler ActiveContentChanged;
#pragma warning restore 0067
			public IViewContent ActiveViewContent
			{
				get { return null; }
			}


			public object ActiveContent
			{
				get { throw new NotImplementedException(); }
			}

			/*
			public IWorkbenchLayout WorkbenchLayout
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{

				}
			}
			*/
			public bool IsActiveWindow
			{
				get { return true; }
			}

			IServiceProvider IWorkbench.ActiveContent => throw new NotImplementedException();

			public string CurrentLayoutConfiguration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

			public void Initialize()
			{

			}

			public void ShowView(IViewContent content)
			{
				throw new NotImplementedException();
			}

			public void ShowView(IViewContent content, bool switchToOpenedView)
			{
				if (!_viewContentCollection.Contains(content))
					_viewContentCollection.Add(content);
			}

			public void ShowPad(ICSharpCode.SharpDevelop.PadDescriptor content)
			{
				throw new NotImplementedException();
			}

			public void UnloadPad(ICSharpCode.SharpDevelop.PadDescriptor content)
			{
				throw new NotImplementedException();
			}

			public ICSharpCode.SharpDevelop.PadDescriptor GetPad(Type type)
			{
				throw new NotImplementedException();
			}

			public void CloseAllViews()
			{
				throw new NotImplementedException();
			}

			public bool CloseAllSolutionViews()
			{
				throw new NotImplementedException();
			}
/*
#pragma warning disable 0067
			public event ViewContentEventHandler ViewOpened;

			public event ViewContentEventHandler ViewClosed;
#pragma warning restore 0067
*/
			private ICollection<IViewContent> _viewContentCollection = new List<IViewContent>();

			public ICSharpCode.Core.Properties CreateMemento()
			{
				throw new NotImplementedException();
			}

			public void SetMemento(ICSharpCode.Core.Properties memento)
			{

			}

			public void ActivatePad(PadDescriptor content)
			{
				throw new NotImplementedException();
			}

			public bool CloseAllSolutionViews(bool force)
			{
				throw new NotImplementedException();
			}

			public System.ComponentModel.ISynchronizeInvoke _synchronizingObject;
			private System.Windows.Window _mainWindow;
			private IStatusBarService _statusBar = new AwdeeCodeStatusBar();
		}


		/*
		class AwdeeCodeWorkbenchLayout : IWorkbenchLayout
		{
			public IWorkbenchWindow ActiveWorkbenchWindow
			{
				get { throw new NotImplementedException(); }
			}

			public IList<IWorkbenchWindow> WorkbenchWindows
			{
				get { throw new NotImplementedException(); }
			}

			public object ActiveContent
			{
				get { throw new NotImplementedException(); }
			}

#pragma warning disable 0067
			public event EventHandler ActiveWorkbenchWindowChanged;

			public event EventHandler ActiveContentChanged;
#pragma warning restore 0067

			public void Attach(IWorkbench workbench)
			{
				throw new NotImplementedException();
			}

			public void Detach()
			{
				throw new NotImplementedException();
			}

			public void ShowPad(ICSharpCode.SharpDevelop.PadDescriptor padDescriptor)
			{
				throw new NotImplementedException();
			}

			public void ActivatePad(ICSharpCode.SharpDevelop.PadDescriptor padDescriptor)
			{
				throw new NotImplementedException();
			}

			public void HidePad(ICSharpCode.SharpDevelop.PadDescriptor padDescriptor)
			{
				throw new NotImplementedException();
			}

			public void UnloadPad(ICSharpCode.SharpDevelop.PadDescriptor padDescriptor)
			{
				throw new NotImplementedException();
			}

			public bool IsVisible(ICSharpCode.SharpDevelop.PadDescriptor padDescriptor)
			{
				throw new NotImplementedException();
			}

			public IWorkbenchWindow ShowView(IViewContent content, bool switchToOpenedView)
			{
				throw new NotImplementedException();
			}

			public void LoadConfiguration()
			{
				throw new NotImplementedException();
			}

			public void StoreConfiguration()
			{
				throw new NotImplementedException();
			}
		}

		*/
		public override void Save()
		{
			if (codeEditor == null) return;
			openedFile.SwitchedToView(content);
			openedFile.SaveToDisk();

		}

		public override void FocusCodeEditor()
		{
			if (codeEditor == null) return;
			host.Focus();
			codeEditor.PrimaryTextEditor.Focus();
		}

		public override string Code
		{
			get
			{
				return codeEditor != null ? codeEditor.PrimaryTextEditor.Text : _codeToLoad;
			}
			set
			{
				if (codeEditor == null)
				{
					if (string.IsNullOrEmpty(value)) return;
					else throw new InvalidOperationException();
				}
				codeEditor.PrimaryTextEditor.Text = value ?? string.Empty;
			}
		}


		private IProject project;

		public override void AddReference(string path)
		{
			path = System.IO.Path.GetFullPath(path);
			var name = System.IO.Path.GetFileNameWithoutExtension(path);
			var f = project.Items.OfType<ReferenceProjectItem>().FirstOrDefault(x => name.Equals( x.Include, StringComparison.OrdinalIgnoreCase));
			if (f == null) ProjectService.AddProjectItem(project, new ReferenceProjectItem(project, name) { HintPath = path });
		}

		public override void RemoveReference(string path)
		{
			var name = System.IO.Path.GetFileNameWithoutExtension(path);

			var f = project.Items.OfType<ReferenceProjectItem>().FirstOrDefault(x => name.Equals(x.Include, StringComparison.OrdinalIgnoreCase));
			if (f != null) ProjectService.RemoveProjectItem(project, f);


		}

		private bool IsPrefetchingInstance;
		private bool _isDirty;
		public override bool IsDirty { get { return _isDirty; } }

		void CodeEditorWrapper_Disposed(object sender, EventArgs e)
		{

		}

	}


	class SynchronizingObjectAdapter : ISynchronizeInvoke
	{
		public SynchronizationContext SyncCtx;
		public int ThreadId;
		public SynchronizingObjectAdapter()
		{
			this.SyncCtx = SynchronizationContext.Current;
			if (SyncCtx == null) throw new InvalidOperationException();
			this.ThreadId = Environment.CurrentManagedThreadId;
		}


		public IAsyncResult BeginInvoke(Delegate method, object[] args)
		{
			object result = null;
			SyncCtx.Post(a => { result = method.DynamicInvoke((object[])args); }, args);
			return null;
		}

		public object EndInvoke(IAsyncResult result)
		{
			throw new NotImplementedException();
		}

		public object Invoke(Delegate method, object[] args)
		{
			object result = null;
			SyncCtx.Send(a => { result = method.DynamicInvoke((object[])args); }, args);
			return result;
		}

		public bool InvokeRequired
		{
			get
			{
				return Environment.CurrentManagedThreadId != ThreadId;
			}
		}
	}
}
