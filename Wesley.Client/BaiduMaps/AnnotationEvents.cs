using Wesley.Client.Models.Terminals;
using System;
namespace Wesley.Client.BaiduMaps
{
    public enum AnnotationDragState
    {
        Starting,
        Dragging,
        Ending
    }

    public class AnnotationDragEventArgs : EventArgs
    {
        public AnnotationDragState State { get; }

        public AnnotationDragEventArgs(AnnotationDragState state)
        {
            State = state;
        }
    }


    public class TagEventArgs : EventArgs
    {
        public TerminalModel Terminal { get; }

        public TagEventArgs(TerminalModel terminal)
        {
            Terminal = terminal;
        }
    }
}

