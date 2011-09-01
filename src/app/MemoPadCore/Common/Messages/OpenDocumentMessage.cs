﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using MemoPadCore.Model;

namespace MemoPadCore.Common.Messages
{
    public class OpenDocumentMessage : NotificationMessage
    {
        public TextDocument Doc { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public OpenDocumentMessage(TextDocument doc)
            : base("open-document-message")
        {
            this.Doc = doc;
        }
    }
}