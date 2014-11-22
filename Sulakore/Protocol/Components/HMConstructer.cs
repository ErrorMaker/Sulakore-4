using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Sulakore.Protocol.Components
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class HMConstructer : ListView
    {
        #region Private Fields
        private ushort _lastHeader;
        private HMessage _packet;
        private readonly List<object> _chunks;
        #endregion

        #region Public Properties
        private HDestinations _destination;
        public HDestinations Destination
        {
            get { return _destination; }
            set
            {
                if (value == HDestinations.Unknown || value == _destination) return;

                _destination = value;
                if (_protocol == HProtocols.Ancient && _chunks.Count > 0)
                    ReconstructList(true);
            }
        }

        private HProtocols _protocol;
        public HProtocols Protocol
        {
            get { return _protocol; }
            set
            {
                if (value == HProtocols.Unknown || value == _protocol) return;

                _protocol = value;
                if (_chunks.Count > 0)
                    ReconstructList(false);
            }
        }

        public bool LockColumns { get; set; }
        #endregion

        #region Constructor(s)
        public HMConstructer()
        {
            _chunks = new List<object>();

            var typeCol = new ColumnHeader { Name = "TypeCol", Text = "Type" };
            var valueCol = new ColumnHeader { Name = "ValueCol", Text = "Value", Width = 194 };
            var encodedCol = new ColumnHeader { Name = "EncodedCol", Text = "Encoded", Width = 105 };
            Columns.AddRange(new[] { typeCol, valueCol, encodedCol });

            FullRowSelect = true;
            GridLines = true;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            MultiSelect = false;
            ShowItemToolTips = true;
            Size = new Size(386, 166);
            UseCompatibleStateImageBehavior = false;
            View = View.Details;
            LockColumns = true;
            _destination = HDestinations.Server;
            _protocol = HProtocols.Modern;
        }
        #endregion

        public void AppendChunk(int value)
        {
            _chunks.Add(value);
            string encoded = HMessage.ToString(HMessage.ConstructBody(Destination, Protocol, value));
            AddItemChunk("Integer", value, encoded);
        }
        public void AppendChunk(bool value)
        {
            _chunks.Add(value);
            string encoded = HMessage.ToString(HMessage.ConstructBody(Destination, Protocol, value));
            AddItemChunk("Boolean", value, encoded);
        }
        public void AppendChunk(string value)
        {
            _chunks.Add(value);

            string encodedLength = string.Empty;
            string encoded = HMessage.ToString(HMessage.ConstructBody(Destination, Protocol, value));
            if (Destination == HDestinations.Server || Protocol == HProtocols.Modern)
            {
                ushort valueLength = (ushort)value.Length;
                byte[] data = Protocol == HProtocols.Ancient ? Ancient.CypherShort(valueLength) : Modern.CypherShort(valueLength);
                encodedLength = HMessage.ToString(data) + " | ";
            }
            AddItemChunk("String", value, encoded, string.Format("Length: {0}{1}\n", encodedLength, value.Length));
        }
        private void AddItemChunk(string type, object value, string encoded, string extraStringInfo = null)
        {
            _lastHeader = 0;
            var item = new ListViewItem(new[] { type, value.ToString(), encoded });
            item.ToolTipText = string.Format("Type: {0}\nValue: {1}\n{2}Encoded: {3}", type, value, extraStringInfo, encoded);

            Focus();
            Items.Add(item);
            item.Selected = true;
            EnsureVisible(Items.Count - 1);
        }

        public void RemoveSelected()
        {
            _lastHeader = 0;
            int index = SelectedIndices[0];
            _chunks.RemoveAt(index);
            Items.RemoveAt(index);

            if (Items.Count > 0)
                Items[index - (index > Items.Count - 1 ? 1 : 0)].Selected = true;
        }
        public void MoveSelectedUp()
        {
            _lastHeader = 0;
            int index = SelectedIndices[0];
            if (index == 0) return;

            object toMoveObj = _chunks[index];
            _chunks.RemoveAt(index);
            _chunks.Insert(index - 1, toMoveObj);

            //Cache SubItems
            var toPushUpItems = new string[4];
            for (int i = 0; i < Items[index].SubItems.Count; i++)
                toPushUpItems[i] = Items[index].SubItems[i].Text;
            toPushUpItems[3] = Items[index].ToolTipText;

            var toPushDownItems = new string[4];
            for (int i = 0; i < Items[index - 1].SubItems.Count; i++)
                toPushDownItems[i] = Items[index - 1].SubItems[i].Text;
            toPushDownItems[3] = Items[index - 1].ToolTipText;

            //Switch
            for (int i = 0; i < 3; i++)
                Items[index].SubItems[i].Text = toPushDownItems[i];
            Items[index].ToolTipText = toPushDownItems[3];

            for (int i = 0; i < 3; i++)
                Items[index - 1].SubItems[i].Text = toPushUpItems[i];
            Items[index - 1].ToolTipText = toPushUpItems[3];

            //Focus / Highlight / Scroll
            Focus();
            Items[index - 1].Selected = true;
            EnsureVisible(index - 1);
        }
        public void MoveSelectedDown()
        {
            _lastHeader = 0;
            int index = SelectedIndices[0];
            if (index == Items.Count - 1) return;

            object toMoveObj = _chunks[index];
            _chunks.RemoveAt(index);
            _chunks.Insert(index + 1, toMoveObj);

            //Cache SubItems
            var toPushDownItems = new string[4];
            for (int i = 0; i < Items[index].SubItems.Count; i++)
                toPushDownItems[i] = Items[index].SubItems[i].Text;
            toPushDownItems[3] = Items[index].ToolTipText;

            var toPushUpItems = new string[4];
            for (int i = 0; i < Items[index + 1].SubItems.Count; i++)
                toPushUpItems[i] = Items[index + 1].SubItems[i].Text;
            toPushUpItems[3] = Items[index + 1].ToolTipText;

            //Switch
            for (int i = 0; i < 3; i++)
                Items[index].SubItems[i].Text = toPushUpItems[i];
            Items[index].ToolTipText = toPushUpItems[3];

            for (int i = 0; i < 3; i++)
                Items[index + 1].SubItems[i].Text = toPushDownItems[i];
            Items[index + 1].ToolTipText = toPushDownItems[3];

            //Focus / Highlight / Scroll
            Focus();
            Items[index + 1].Selected = true;
            EnsureVisible(index + 1);
        }
        public void ReplaceSelected(object value)
        {
            _lastHeader = 0;
            int index = SelectedIndices[0];
            if (value.Equals(_chunks[index])) return;

            _chunks[index] = value;
            ListViewItem curItem = Items[index];
            string type = value is string ? "String" : value is int ? "Integer" : "Boolean";
            string encoded = HMessage.ToString(HMessage.ConstructBody(Destination, Protocol, value));
            curItem.SubItems[0].Text = type;
            curItem.SubItems[1].Text = value.ToString();
            curItem.SubItems[2].Text = encoded;
            string encodedLength = string.Empty;

            if (value is string)
            {
                if ((Destination == HDestinations.Server && Protocol == HProtocols.Ancient) || Protocol == HProtocols.Modern)
                {
                    ushort valueLength = (ushort)value.ToString().Length;
                    byte[] data = Protocol == HProtocols.Ancient ? Ancient.CypherShort(valueLength) : Modern.CypherShort(valueLength);
                    encodedLength = HMessage.ToString(data) + " | ";
                }
            }
            curItem.ToolTipText = string.Format("Type: {0}\nValue: {1}\n{2}Encoded: {3}", type, value, string.Format("Length: {0}{1}\n", encodedLength, value.ToString().Length), encoded);
        }

        public void ClearChunks()
        {
            _chunks.Clear();
            Items.Clear();
        }
        public HMessage Construct(ushort header)
        {
            if (_lastHeader == header) return _packet;
            _lastHeader = header;
            return _packet = new HMessage(header, Destination, Protocol, _chunks.ToArray());
        }
        private void ReconstructList(bool stringsOnly)
        {
            BeginUpdate();
            for (int i = 0; i < _chunks.Count; i++)
            {
                object chunk = _chunks[i];
                if (!(chunk is string) && stringsOnly) continue;
                string encoded = HMessage.ToString(HMessage.ConstructBody(Destination, Protocol, chunk));
                Items[i].SubItems[2].Text = encoded;

                var value = chunk as string;
                if (value != null)
                {
                    string encodedLength = string.Empty;
                    if ((Destination == HDestinations.Server && Protocol == HProtocols.Ancient) || Protocol == HProtocols.Modern)
                    {
                        ushort valueLength = (ushort)value.Length;
                        byte[] data = Protocol == HProtocols.Ancient ? Ancient.CypherShort(valueLength) : Modern.CypherShort(valueLength);
                        encodedLength = HMessage.ToString(data) + " | ";
                    }

                    Items[i].ToolTipText = string.Format("Type: String\nValue: {0}\n{1}Encoded: {2}", value, string.Format("Length: {0}{1}\n", encodedLength, value.Length), encoded);
                }
                else Items[i].ToolTipText = Items[i].ToolTipText.Replace(Items[i].ToolTipText.GetChild("Encoded: ", '\n'), encoded);
            }
            EndUpdate();
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            if (LockColumns)
            {
                e.Cancel = true;
                e.NewWidth = Columns[e.ColumnIndex].Width;
            }
            base.OnColumnWidthChanging(e);
        }
    }
}