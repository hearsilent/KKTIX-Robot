<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KKTIX_Robot
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    '為 Windows Form 設計工具的必要項
    Private components As System.ComponentModel.IContainer

    '注意:  以下為 Windows Form 設計工具所需的程序
    '可以使用 Windows Form 設計工具進行修改。
    '請不要使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KKTIX_Robot))
        Me.reCAPTCHA = New System.Windows.Forms.PictureBox()
        Me.SendButton = New System.Windows.Forms.Button()
        Me.reCaptchaTextBox = New System.Windows.Forms.TextBox()
        Me.TicketComboBox = New System.Windows.Forms.ComboBox()
        Me.KKTIX_URL = New System.Windows.Forms.TextBox()
        Me.WebLabel = New System.Windows.Forms.Label()
        Me.GetButton = New System.Windows.Forms.Button()
        Me.reCaptchaLabel = New System.Windows.Forms.Label()
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.CheckBox = New System.Windows.Forms.CheckBox()
        CType(Me.reCAPTCHA, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'reCAPTCHA
        '
        Me.reCAPTCHA.Location = New System.Drawing.Point(12, 100)
        Me.reCAPTCHA.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.reCAPTCHA.Name = "reCAPTCHA"
        Me.reCAPTCHA.Size = New System.Drawing.Size(300, 57)
        Me.reCAPTCHA.TabIndex = 1
        Me.reCAPTCHA.TabStop = False
        '
        'SendButton
        '
        Me.SendButton.Enabled = False
        Me.SendButton.Location = New System.Drawing.Point(118, 165)
        Me.SendButton.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.SendButton.Name = "SendButton"
        Me.SendButton.Size = New System.Drawing.Size(87, 31)
        Me.SendButton.TabIndex = 2
        Me.SendButton.Text = "送出"
        Me.SendButton.UseVisualStyleBackColor = True
        '
        'reCaptchaTextBox
        '
        Me.reCaptchaTextBox.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.reCaptchaTextBox.Location = New System.Drawing.Point(65, 69)
        Me.reCaptchaTextBox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.reCaptchaTextBox.Name = "reCaptchaTextBox"
        Me.reCaptchaTextBox.Size = New System.Drawing.Size(247, 23)
        Me.reCaptchaTextBox.TabIndex = 3
        '
        'TicketComboBox
        '
        Me.TicketComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.TicketComboBox.FormattingEnabled = True
        Me.TicketComboBox.Location = New System.Drawing.Point(14, 37)
        Me.TicketComboBox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.TicketComboBox.Name = "TicketComboBox"
        Me.TicketComboBox.Size = New System.Drawing.Size(298, 24)
        Me.TicketComboBox.TabIndex = 4
        '
        'KKTIX_URL
        '
        Me.KKTIX_URL.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.KKTIX_URL.Location = New System.Drawing.Point(82, 6)
        Me.KKTIX_URL.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.KKTIX_URL.Name = "KKTIX_URL"
        Me.KKTIX_URL.Size = New System.Drawing.Size(164, 23)
        Me.KKTIX_URL.TabIndex = 5
        Me.KKTIX_URL.Text = "http://hitcon.kktix.cc/events/hitcon-x-plg"
        '
        'WebLabel
        '
        Me.WebLabel.AutoSize = True
        Me.WebLabel.Location = New System.Drawing.Point(11, 9)
        Me.WebLabel.Name = "WebLabel"
        Me.WebLabel.Size = New System.Drawing.Size(65, 16)
        Me.WebLabel.TabIndex = 6
        Me.WebLabel.Text = "搶票網址 : "
        '
        'GetButton
        '
        Me.GetButton.Location = New System.Drawing.Point(252, 6)
        Me.GetButton.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.GetButton.Name = "GetButton"
        Me.GetButton.Size = New System.Drawing.Size(60, 23)
        Me.GetButton.TabIndex = 7
        Me.GetButton.Text = "獲取"
        Me.GetButton.UseVisualStyleBackColor = True
        '
        'reCaptchaLabel
        '
        Me.reCaptchaLabel.AutoSize = True
        Me.reCaptchaLabel.Location = New System.Drawing.Point(11, 72)
        Me.reCaptchaLabel.Name = "reCaptchaLabel"
        Me.reCaptchaLabel.Size = New System.Drawing.Size(53, 16)
        Me.reCaptchaLabel.TabIndex = 8
        Me.reCaptchaLabel.Text = "驗證碼 : "
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 69)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(324, 22)
        Me.StatusStrip.TabIndex = 9
        Me.StatusStrip.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel
        '
        Me.ToolStripStatusLabel.Font = New System.Drawing.Font("微軟正黑體", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.ToolStripStatusLabel.ForeColor = System.Drawing.Color.DarkOrange
        Me.ToolStripStatusLabel.Name = "ToolStripStatusLabel"
        Me.ToolStripStatusLabel.Size = New System.Drawing.Size(104, 17)
        Me.ToolStripStatusLabel.Text = "程式尚未開始搶票"
        '
        'CheckBox
        '
        Me.CheckBox.AutoSize = True
        Me.CheckBox.Enabled = False
        Me.CheckBox.ForeColor = System.Drawing.Color.DarkOrange
        Me.CheckBox.Location = New System.Drawing.Point(252, 72)
        Me.CheckBox.Name = "CheckBox"
        Me.CheckBox.Size = New System.Drawing.Size(51, 20)
        Me.CheckBox.TabIndex = 10
        Me.CheckBox.Text = "啟動"
        Me.CheckBox.UseVisualStyleBackColor = True
        '
        'KKTIX_Robot
        '
        Me.AcceptButton = Me.SendButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(324, 91)
        Me.Controls.Add(Me.CheckBox)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.reCaptchaLabel)
        Me.Controls.Add(Me.GetButton)
        Me.Controls.Add(Me.WebLabel)
        Me.Controls.Add(Me.KKTIX_URL)
        Me.Controls.Add(Me.TicketComboBox)
        Me.Controls.Add(Me.reCaptchaTextBox)
        Me.Controls.Add(Me.SendButton)
        Me.Controls.Add(Me.reCAPTCHA)
        Me.Font = New System.Drawing.Font("微軟正黑體", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "KKTIX_Robot"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KKTIX Robot"
        CType(Me.reCAPTCHA, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents reCAPTCHA As System.Windows.Forms.PictureBox
    Friend WithEvents SendButton As System.Windows.Forms.Button
    Friend WithEvents reCaptchaTextBox As System.Windows.Forms.TextBox
    Friend WithEvents TicketComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents KKTIX_URL As System.Windows.Forms.TextBox
    Friend WithEvents WebLabel As System.Windows.Forms.Label
    Friend WithEvents GetButton As System.Windows.Forms.Button
    Friend WithEvents reCaptchaLabel As System.Windows.Forms.Label
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents CheckBox As System.Windows.Forms.CheckBox

End Class
