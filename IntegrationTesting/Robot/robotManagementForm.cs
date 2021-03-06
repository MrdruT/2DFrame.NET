﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntegrationTesting.Robot
{
    public partial class RobotManagementForm : Form
    {
        GRpcServer m_connectToRobot = null;
        public RobotManagementForm()
        {
            InitializeComponent();
        }

        private void buttonServerStart_Click(object sender, EventArgs e)
        {
            if(m_connectToRobot == null)
            {
                m_connectToRobot = new GRpcServer();
            }
            m_connectToRobot.ServerStart();
        }
    }
}
