using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XecMeConfig.Controls
{
    public class CheckedComboBox : ComboBox
    {
        private Panel _panel;
        public CheckedComboBox()
        {
            _panel = new Panel();
            this.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
        }

        protected override void OnDropDown(EventArgs e)
        {
            _panel.Visible = true;
            base.OnDropDown(e);
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            //CheckedListBox.ObjectCollection
            _panel.Visible = false;
            base.OnDropDownClosed(e);
        }


    }

    public static class Extensions
    {
        // Summary:
        //     Adds an item to the list of items for a System.Windows.Forms.CheckedListBox,
        //     specifying the object to add and whether it is checked.
        //
        // Parameters:
        //   item:
        //     An object representing the item to add to the collection.
        //
        //   isChecked:
        //     true to check the item; otherwise, false.
        //
        // Returns:
        //     The index of the newly added item.
        public static int Add(this CheckedComboBox cmbBox, object item, bool isChecked)
        {
            return 0;
        }
        //
        // Summary:
        //     Adds an item to the list of items for a System.Windows.Forms.CheckedListBox,
        //     specifying the object to add and the initial checked value.
        //
        // Parameters:
        //   item:
        //     An object representing the item to add to the collection.
        //
        //   check:
        //     The initial System.Windows.Forms.CheckState for the checked portion of the
        //     item.
        //
        // Returns:
        //     The index of the newly added item.
        //
        // Exceptions:
        //   System.ComponentModel.InvalidEnumArgumentException:
        //     The check parameter is not one of the valid System.Windows.Forms.CheckState
        //     values.
        public static int Add(this CheckedComboBox cmbBox, object item, CheckState check)
        {
            return 0;
        }
    }



}
