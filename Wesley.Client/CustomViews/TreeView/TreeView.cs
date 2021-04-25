using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;

namespace Wesley.Client.CustomViews
{
    public class TreeView : ScrollView
    {
        #region Fields

        private readonly StackLayout _StackLayout = new StackLayout { Orientation = StackOrientation.Vertical };
        private TreeViewNode _SelectedItem;

        public static readonly BindableProperty RootNodesProperty = BindableProperty.Create(
            nameof(IList<TreeViewNode>),
            typeof(IList<TreeViewNode>),
            typeof(TreeView),
            default(List<TreeViewNode>));

        #endregion

        #region Public Properties

        //private static void RootNodesChanged(BindableObject bindable, object oldvalue, object newvalue)
        //{
        //    var rootNodes = (TreeView)bindable;
        //    if (rootNodes != null)
        //    {
        //        var newvalues = (ICollection)newvalue;
        //        var oldvalues = (ICollection)oldvalue;
        //        if (newvalues != null)
        //        {
        //            var items = new List<TreeViewNode>();
        //            foreach (var ss in newvalues)
        //            {
        //                var item = ss as TreeViewNode;
        //                items.Add(item);
        //            }
        //            if (items.Any())
        //            {
        //                rootNodes.SetBindings(items);
        //            }
        //        }
        //        else if (oldvalues != null)
        //        {
        //            var items = new List<TreeViewNode>();
        //            foreach (var ss in oldvalues)
        //            {
        //                var item = ss as TreeViewNode;
        //                items.Add(item);
        //            }
        //            if (items.Any())
        //            {
        //                rootNodes.SetBindings(items);
        //            }
        //        }
        //    }
        //}


        private void SetBindings(List<TreeViewNode> treeViewNodes)
        {
            if (treeViewNodes != null && treeViewNodes.Any())
            {
                this.SetBinding(RootNodesProperty, new Binding("RootNodes", source: treeViewNodes));
            }
        }

        public IList<TreeViewNode> RootNodes
        {
            get { return (IList<TreeViewNode>)GetValue(RootNodesProperty); }
            set
            {
                SetValue(RootNodesProperty, value);

                if (value is INotifyCollectionChanged notifyCollectionChanged)
                {
                    notifyCollectionChanged.CollectionChanged += (s, e) =>
                    {
                        RenderNodes(value, _StackLayout, e, null);
                    };
                }
                RenderNodes(value, _StackLayout, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), null);
            }
        }


        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

        }


        /// <summary>
        /// The item that is selected in the tree
        /// TODO: Make this two way - and maybe eventually a bindable property
        /// </summary>
        public TreeViewNode SelectedItem
        {
            get => _SelectedItem;

            set
            {
                if (_SelectedItem == value)
                {
                    return;
                }

                if (_SelectedItem != null)
                {
                    _SelectedItem.IsSelected = false;
                }

                _SelectedItem = value;

                SelectedItemChanged?.Invoke(this, new EventArgs());
            }
        }




        #endregion

        #region Events
        /// <summary>
        /// Occurs when the user selects a TreeViewItem
        /// </summary>
        public event EventHandler SelectedItemChanged;

        #endregion

        #region Constructor
        public TreeView()
        {
            Content = _StackLayout;
            this.SetBinding(RootNodesProperty, new Binding(nameof(RootNodes)));
        }
        #endregion

        #region Private Methods
        private void RemoveSelectionRecursive(IEnumerable<TreeViewNode> nodes)
        {
            foreach (var treeViewItem in nodes)
            {
                if (treeViewItem != SelectedItem)
                {
                    treeViewItem.IsSelected = false;
                }

                RemoveSelectionRecursive(treeViewItem.Children);
            }
        }
        #endregion

        #region Private Static Methods
        private static void AddItems(IEnumerable<TreeViewNode> childTreeViewItems, StackLayout parent, TreeViewNode parentTreeViewItem)
        {
            foreach (var childTreeNode in childTreeViewItems)
            {
                if (!parent.Children.Contains(childTreeNode))
                {
                    parent.Children.Add(childTreeNode);
                }

                childTreeNode.ParentTreeViewItem = parentTreeViewItem;
            }
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// TODO: A bit stinky but better than bubbling an event up...
        /// </summary>
        internal void ChildSelected(TreeViewNode child)
        {
            SelectedItem = child;
            //child.IsSelected = !child.IsSelected;
            //child.SelectionBoxView.Color = child.SelectedBackgroundColor;
            //child.SelectionBoxView.Opacity = child.SelectedBackgroundOpacity;

            //if (child.BindingContext is XamlItem vm)
            //    vm.Selected = !vm.Selected;

            RemoveSelectionRecursive(RootNodes);
        }
        #endregion

        #region Internal Static Methods
        internal static void RenderNodes(IEnumerable<TreeViewNode> childTreeViewItems, StackLayout parent, NotifyCollectionChangedEventArgs e, TreeViewNode parentTreeViewItem)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                //TODO: Reintate this...
                //parent.Children.Clear();
                AddItems(childTreeViewItems, parent, parentTreeViewItem);
            }
            else
            {
                AddItems(e.NewItems.Cast<TreeViewNode>(), parent, parentTreeViewItem);
            }
        }
        #endregion
    }
}