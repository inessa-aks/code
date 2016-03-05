using AndroidXml;
using AndroidXml.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AndroidXml.Res
{
    public class ResXMLParser
    {
        private List<ResXMLTree_attribute> _attributes;
        private object _currentExtension;
        private ResXMLTree_node _currentNode;
        private XmlParserEventCode _eventCode;
        private readonly IEnumerator<XmlParserEventCode> _parserIterator;
        private ResReader _reader;
        private ResResourceMap _resourceMap;
        private readonly Stream _source;
        private ResStringPool _strings;

        public ResXMLParser(Stream source)
        {
            this._source = source;
            this._reader = new ResReader(this._source);
            this._eventCode = XmlParserEventCode.NOT_STARTED;
            this._parserIterator = this.ParserIterator().GetEnumerator();
        }

        private void ClearState()
        {
            this._currentNode = null;
            this._currentExtension = null;
            this._attributes = null;
        }

        public void Close()
        {
            if (this._eventCode != XmlParserEventCode.CLOSED)
            {
                this._eventCode = XmlParserEventCode.CLOSED;
                this._reader.Close();
            }
        }

        public AttributeInfo GetAttribute(uint? index)
        {
            if (!index.HasValue || (this._attributes == null))
            {
                return null;
            }
            uint? nullable2 = index;
            long? nullable = nullable2.HasValue ? new long?((long)((ulong)nullable2.GetValueOrDefault())) : null;
            long count = this._attributes.Count;
            if ((nullable.GetValueOrDefault() >= count) ? nullable.HasValue : false)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            return new AttributeInfo(this, this._attributes[index.Value]);
        }

        public uint? IndexOfAttribute(string ns, string attribute)
        {
            uint? nullable = this._strings.IndexOfString(ns);
            uint? nullable2 = this._strings.IndexOfString(attribute);
            if (nullable2.HasValue)
            {
                uint num = 0;
                foreach (ResXMLTree_attribute _attribute in this._attributes)
                {
                    uint? index = _attribute.Namespace.Index;
                    uint? nullable4 = nullable;
                    if ((index.GetValueOrDefault() == nullable4.GetValueOrDefault()) ? (index.HasValue == nullable4.HasValue) : false)
                    {
                        nullable4 = _attribute.Name.Index;
                        index = nullable2;
                        if ((nullable4.GetValueOrDefault() == index.GetValueOrDefault()) ? (nullable4.HasValue == index.HasValue) : false)
                        {
                            return new uint?(num);
                        }
                    }
                    num++;
                }
            }
            return null;
        }

        public XmlParserEventCode Next()
        {
            if (this._parserIterator.MoveNext())
            {
                this._eventCode = this._parserIterator.Current;
                return this._parserIterator.Current;
            }
            this._eventCode = XmlParserEventCode.END_DOCUMENT;
            return this._eventCode;
        }

        private IEnumerable<XmlParserEventCode> ParserIterator()
        {
            Label_0032:
            this.ClearState();
            try
            {
                this.< header > 5__1 = this._reader.ReadResChunk_header();
            }
            catch (EndOfStreamException)
            {
            }
            this.< subStream > 5__3 = new BoundedStream(this._reader.BaseStream, (long)(this.< header > 5__1.Size - 8));
            this.< subReader > 5__2 = new ResReader(this.< subStream > 5__3);
            switch (this.< header > 5__1.Type)
            {
                case ResourceType.RES_XML_START_NAMESPACE_TYPE:
                    this._currentNode = this.< subReader > 5__2.ReadResXMLTree_node(this.< header > 5__1);
                this._currentExtension = this.< subReader > 5__2.ReadResXMLTree_namespaceExt();
                yield return XmlParserEventCode.START_NAMESPACE;
                break;

                case ResourceType.RES_XML_END_NAMESPACE_TYPE:
                    this._currentNode = this.< subReader > 5__2.ReadResXMLTree_node(this.< header > 5__1);
                this._currentExtension = this.< subReader > 5__2.ReadResXMLTree_namespaceExt();
                yield return XmlParserEventCode.END_NAMESPACE;
                break;

                case ResourceType.RES_XML_START_ELEMENT_TYPE:
                {
                    this._currentNode = this.< subReader > 5__2.ReadResXMLTree_node(this.< header > 5__1);
                    ResXMLTree_attrExt ext = this.< subReader > 5__2.ReadResXMLTree_attrExt();
                    this._currentExtension = ext;
                    this._attributes = new List<ResXMLTree_attribute>();
                    for (int j = 0; j < ext.AttributeCount; j++)
                    {
                        this._attributes.Add(this.< subReader > 5__2.ReadResXMLTree_attribute());
                    }
                    yield return XmlParserEventCode.START_TAG;
                    break;
                }
                case ResourceType.RES_XML_END_ELEMENT_TYPE:
                    this._currentNode = this.< subReader > 5__2.ReadResXMLTree_node(this.< header > 5__1);
                this._currentExtension = this.< subReader > 5__2.ReadResXMLTree_endElementExt();
                yield return XmlParserEventCode.END_TAG;
                break;

                case ResourceType.RES_XML_CDATA_TYPE:
                    this._currentNode = this.< subReader > 5__2.ReadResXMLTree_node(this.< header > 5__1);
                this._currentExtension = this.< subReader > 5__2.ReadResXMLTree_cdataExt();
                yield return XmlParserEventCode.TEXT;
                break;

                case ResourceType.RES_XML_RESOURCE_MAP_TYPE:
                {
                    ResResourceMap map = this.< subReader > 5__2.ReadResResourceMap(this.< header > 5__1);
                    this._resourceMap = map;
                    break;
                }
                case ResourceType.RES_STRING_POOL_TYPE:
                {
                    ResStringPool_header header = this.< subReader > 5__2.ReadResStringPool_header(this.< header > 5__1);
                    this._strings = this.< subReader > 5__2.ReadResStringPool(header);
                    break;
                }
                case ResourceType.RES_XML_TYPE:
                    yield return XmlParserEventCode.START_DOCUMENT;
                this._reader = this.< subReader > 5__2;
                goto Label_0032;

                default:
                    Console.WriteLine("Warning: Skipping chunk of type {0} (0x{1:x4})", this.< header > 5__1.Type, (int)this.< header > 5__1.Type);
                break;
            }
            byte[] buffer = this.< subStream > 5__3.ReadFully();
            if (buffer.Length != 0)
            {
                Console.WriteLine("Warning: Skipping {0} bytes at the end of a {1} (0x{2:x4}) chunk.", buffer.Length, this.< header > 5__1.Type, (int)this.< header > 5__1.Type);
            }
            this.< header > 5__1 = null;
            this.< subStream > 5__3 = null;
            this.< subReader > 5__2 = null;
            goto Label_0032;
        }

        public void Restart()
        {
            throw new NotSupportedException();
        }

        public uint AttributeCount
        {
            get
            {
                if (this._attributes != null)
                {
                    return (uint)this._attributes.Count;
                }
                return 0;
            }
        }

        public string CData
        {
            get
            {
                return this.Strings.GetString(this.CDataID);
            }
        }

        public uint? CDataID
        {
            get
            {
                ResXMLTree_cdataExt ext = this._currentExtension as ResXMLTree_cdataExt;
                if (ext != null)
                {
                    return ext.Data.Index;
                }
                return null;
            }
        }

        public string Comment
        {
            get
            {
                return this._strings.GetString(this.CommentID);
            }
        }

        public uint? CommentID
        {
            get
            {
                if (this._currentNode != null)
                {
                    return this._currentNode.Comment.Index;
                }
                return null;
            }
        }

        public AttributeInfo ElementClass
        {
            get
            {
                return this.GetAttribute(this.ElementClassIndex);
            }
        }

        public uint? ElementClassIndex
        {
            get
            {
                ResXMLTree_attrExt ext = this._currentExtension as ResXMLTree_attrExt;
                if (ext != null)
                {
                    return new uint?(ext.ClassIndex);
                }
                return null;
            }
        }

        public AttributeInfo ElementId
        {
            get
            {
                return this.GetAttribute(this.ElementIdIndex);
            }
        }

        public uint? ElementIdIndex
        {
            get
            {
                ResXMLTree_attrExt ext = this._currentExtension as ResXMLTree_attrExt;
                if (ext != null)
                {
                    return new uint?(ext.IdIndex);
                }
                return null;
            }
        }

        public string ElementName
        {
            get
            {
                return this.Strings.GetString(this.ElementNameID);
            }
        }

        public uint? ElementNameID
        {
            get
            {
                ResXMLTree_attrExt ext = this._currentExtension as ResXMLTree_attrExt;
                if (ext != null)
                {
                    return ext.Name.Index;
                }
                ResXMLTree_endElementExt ext2 = this._currentExtension as ResXMLTree_endElementExt;
                if (ext2 != null)
                {
                    return ext2.Name.Index;
                }
                return null;
            }
        }

        public string ElementNamespace
        {
            get
            {
                return this.Strings.GetString(this.ElementNamespaceID);
            }
        }

        public uint? ElementNamespaceID
        {
            get
            {
                ResXMLTree_attrExt ext = this._currentExtension as ResXMLTree_attrExt;
                if (ext != null)
                {
                    return ext.Namespace.Index;
                }
                ResXMLTree_endElementExt ext2 = this._currentExtension as ResXMLTree_endElementExt;
                if (ext2 != null)
                {
                    return ext2.Namespace.Index;
                }
                return null;
            }
        }

        public AttributeInfo ElementStyle
        {
            get
            {
                return this.GetAttribute(this.ElementStyleIndex);
            }
        }

        public uint? ElementStyleIndex
        {
            get
            {
                ResXMLTree_attrExt ext = this._currentExtension as ResXMLTree_attrExt;
                if (ext != null)
                {
                    return new uint?(ext.StyleIndex);
                }
                return null;
            }
        }

        public XmlParserEventCode EventCode
        {
            get
            {
                return this._eventCode;
            }
        }

        public uint? LineNumber
        {
            get
            {
                if (this._currentNode != null)
                {
                    return new uint?(this._currentNode.LineNumber);
                }
                return null;
            }
        }

        public string NamespacePrefix
        {
            get
            {
                return this.Strings.GetString(this.NamespacePrefixID);
            }
        }

        public uint? NamespacePrefixID
        {
            get
            {
                ResXMLTree_namespaceExt ext = this._currentExtension as ResXMLTree_namespaceExt;
                if (ext != null)
                {
                    return ext.Prefix.Index;
                }
                return null;
            }
        }

        public string NamespaceUri
        {
            get
            {
                return this.Strings.GetString(this.NamespaceUriID);
            }
        }

        public uint? NamespaceUriID
        {
            get
            {
                ResXMLTree_namespaceExt ext = this._currentExtension as ResXMLTree_namespaceExt;
                if (ext != null)
                {
                    return ext.Uri.Index;
                }
                return null;
            }
        }

        public ResResourceMap ResourceMap
        {
            get
            {
                return this._resourceMap;
            }
        }

        public ResStringPool Strings
        {
            get
            {
                return this._strings;
            }
        }

        [CompilerGenerated]
        private sealed class <ParserIterator>d__60 : IEnumerable<ResXMLParser.XmlParserEventCode>, IEnumerable, IEnumerator<ResXMLParser.XmlParserEventCode>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private ResXMLParser.XmlParserEventCode<>2__current;
            public ResXMLParser<>4__this;
            private int <>l__initialThreadId;
            private ResChunk_header<header>5__1;
            private ResReader<subReader>5__2;
            private BoundedStream<subStream>5__3;

            [DebuggerHidden]
        public <ParserIterator>d__60(int <>1__state)
        {
            this.<> 1__state = <> 1__state;
            this.<> l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        private bool MoveNext()
        {
            byte[] buffer;
            switch (this.<> 1__state)
                {
                    case 0:
                        this.<> 1__state = -1;
                break;

                    case 1:
                        this.<> 1__state = -1;
                this.<> 4__this._reader = this.< subReader > 5__2;
                break;

                    case 2:
                        this.<> 1__state = -1;
                goto Label_0362;

                    case 3:
                        this.<> 1__state = -1;
                goto Label_0362;

                    case 4:
                        this.<> 1__state = -1;
                goto Label_0362;

                    case 5:
                        this.<> 1__state = -1;
                goto Label_0362;

                    case 6:
                        this.<> 1__state = -1;
                goto Label_0362;

                default:
                        return false;
            }
            Label_0032:
            this.<> 4__this.ClearState();
            try
            {
                this.< header > 5__1 = this.<> 4__this._reader.ReadResChunk_header();
            }
            catch (EndOfStreamException)
            {
                return false;
            }
            this.< subStream > 5__3 = new BoundedStream(this.<> 4__this._reader.BaseStream, (long)(this.< header > 5__1.Size - 8));
            this.< subReader > 5__2 = new ResReader(this.< subStream > 5__3);
            switch (this.< header > 5__1.Type)
                {
                    case ResourceType.RES_XML_START_NAMESPACE_TYPE:
                        this.<> 4__this._currentNode = this.< subReader > 5__2.ReadResXMLTree_node(this.< header > 5__1);
                this.<> 4__this._currentExtension = this.< subReader > 5__2.ReadResXMLTree_namespaceExt();
                this.<> 2__current = ResXMLParser.XmlParserEventCode.START_NAMESPACE;
                this.<> 1__state = 2;
                return true;

                    case ResourceType.RES_XML_END_NAMESPACE_TYPE:
                        this.<> 4__this._currentNode = this.< subReader > 5__2.ReadResXMLTree_node(this.< header > 5__1);
                this.<> 4__this._currentExtension = this.< subReader > 5__2.ReadResXMLTree_namespaceExt();
                this.<> 2__current = ResXMLParser.XmlParserEventCode.END_NAMESPACE;
                this.<> 1__state = 3;
                return true;

                    case ResourceType.RES_XML_START_ELEMENT_TYPE:
                    {
                    this.<> 4__this._currentNode = this.< subReader > 5__2.ReadResXMLTree_node(this.< header > 5__1);
                    ResXMLTree_attrExt ext = this.< subReader > 5__2.ReadResXMLTree_attrExt();
                    this.<> 4__this._currentExtension = ext;
                    this.<> 4__this._attributes = new List<ResXMLTree_attribute>();
                    for (int i = 0; i < ext.AttributeCount; i++)
                    {
                        this.<> 4__this._attributes.Add(this.< subReader > 5__2.ReadResXMLTree_attribute());
                    }
                    this.<> 2__current = ResXMLParser.XmlParserEventCode.START_TAG;
                    this.<> 1__state = 4;
                    return true;
                }
                    case ResourceType.RES_XML_END_ELEMENT_TYPE:
                        this.<> 4__this._currentNode = this.< subReader > 5__2.ReadResXMLTree_node(this.< header > 5__1);
                this.<> 4__this._currentExtension = this.< subReader > 5__2.ReadResXMLTree_endElementExt();
                this.<> 2__current = ResXMLParser.XmlParserEventCode.END_TAG;
                this.<> 1__state = 5;
                return true;

                    case ResourceType.RES_XML_CDATA_TYPE:
                        this.<> 4__this._currentNode = this.< subReader > 5__2.ReadResXMLTree_node(this.< header > 5__1);
                this.<> 4__this._currentExtension = this.< subReader > 5__2.ReadResXMLTree_cdataExt();
                this.<> 2__current = ResXMLParser.XmlParserEventCode.TEXT;
                this.<> 1__state = 6;
                return true;

                    case ResourceType.RES_XML_RESOURCE_MAP_TYPE:
                    {
                    ResResourceMap map = this.< subReader > 5__2.ReadResResourceMap(this.< header > 5__1);
                    this.<> 4__this._resourceMap = map;
                    break;
                }
                    case ResourceType.RES_STRING_POOL_TYPE:
                    {
                    ResStringPool_header header = this.< subReader > 5__2.ReadResStringPool_header(this.< header > 5__1);
                    this.<> 4__this._strings = this.< subReader > 5__2.ReadResStringPool(header);
                    break;
                }
                    case ResourceType.RES_XML_TYPE:
                        this.<> 2__current = ResXMLParser.XmlParserEventCode.START_DOCUMENT;
                this.<> 1__state = 1;
                return true;

                default:
                        Console.WriteLine("Warning: Skipping chunk of type {0} (0x{1:x4})", this.< header > 5__1.Type, (int)this.< header > 5__1.Type);
                break;
            }
            Label_0362:
            buffer = this.< subStream > 5__3.ReadFully();
            if (buffer.Length != 0)
            {
                Console.WriteLine("Warning: Skipping {0} bytes at the end of a {1} (0x{2:x4}) chunk.", buffer.Length, this.< header > 5__1.Type, (int)this.< header > 5__1.Type);
            }
            this.< header > 5__1 = null;
            this.< subStream > 5__3 = null;
            this.< subReader > 5__2 = null;
            goto Label_0032;
        }

        [DebuggerHidden]
        IEnumerator<ResXMLParser.XmlParserEventCode> IEnumerable<ResXMLParser.XmlParserEventCode>.GetEnumerator()
        {
            if ((this.<> 1__state == -2) && (this.<> l__initialThreadId == Thread.CurrentThread.ManagedThreadId))
                {
                this.<> 1__state = 0;
                return this;
            }
            return new ResXMLParser.< ParserIterator > d__60(0) { <> 4__this = this.<> 4__this };
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.System.Collections.Generic.IEnumerable<AndroidXml.Res.ResXMLParser.XmlParserEventCode>.GetEnumerator();
        }

        [DebuggerHidden]
        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        void IDisposable.Dispose()
        {
        }

        ResXMLParser.XmlParserEventCode IEnumerator<ResXMLParser.XmlParserEventCode>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.<> 2__current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.<> 2__current;
            }
        }
    }

    public class AttributeInfo
    {
        private readonly ResXMLParser _parser;

        public AttributeInfo(ResXMLParser parser, ResXMLTree_attribute attribute)
        {
            this._parser = parser;
            this.TypedValue = attribute.TypedValue;
            this.ValueStringID = attribute.RawValue.Index;
            this.NameID = attribute.Name.Index;
            this.NamespaceID = attribute.Namespace.Index;
        }

        public string Name
        {
            get
            {
                return this._parser.Strings.GetString(this.NameID);
            }
        }

        public uint? NameID { get; private set; }

        public string Namespace
        {
            get
            {
                return this._parser.Strings.GetString(this.NamespaceID);
            }
        }

        public uint? NamespaceID { get; private set; }

        public Res_value TypedValue { get; private set; }

        public string ValueString
        {
            get
            {
                return this._parser.Strings.GetString(this.ValueStringID);
            }
        }

        public uint? ValueStringID { get; private set; }
    }

    public enum XmlParserEventCode
    {
        BAD_DOCUMENT = 1,
        CLOSED = 4,
        END_DOCUMENT = 3,
        END_NAMESPACE = 0x101,
        END_TAG = 0x103,
        NOT_STARTED = 0,
        START_DOCUMENT = 2,
        START_NAMESPACE = 0x100,
        START_TAG = 0x102,
        TEXT = 260
    }
}
