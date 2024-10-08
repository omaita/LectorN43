<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class mainFrm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(mainFrm))
        btnOpen = New Button()
        btnClose = New Button()
        txtBoxPrint = New TextBox()
        btnPreview = New Button()
        btnPrint = New Button()
        btnExportCSV = New Button()
        panelTextBox = New Panel()
        panelTextBox.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnOpen
        ' 
        btnOpen.AutoSize = True
        btnOpen.Location = New Point(24, 26)
        btnOpen.Name = "btnOpen"
        btnOpen.Size = New Size(152, 29)
        btnOpen.TabIndex = 0
        btnOpen.Text = "Abrir Archivo"
        btnOpen.UseVisualStyleBackColor = True
        ' 
        ' btnClose
        ' 
        btnClose.AutoSize = True
        btnClose.Location = New Point(24, 372)
        btnClose.Name = "btnClose"
        btnClose.Size = New Size(152, 29)
        btnClose.TabIndex = 4
        btnClose.Text = "Salir"
        btnClose.UseVisualStyleBackColor = True
        ' 
        ' txtBoxPrint
        ' 
        txtBoxPrint.Dock = DockStyle.Fill
        txtBoxPrint.Font = New Font("Courier New", 10F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        txtBoxPrint.Location = New Point(0, 0)
        txtBoxPrint.Margin = New Padding(2)
        txtBoxPrint.MaxLength = 0
        txtBoxPrint.Multiline = True
        txtBoxPrint.Name = "txtBoxPrint"
        txtBoxPrint.ReadOnly = True
        txtBoxPrint.ScrollBars = ScrollBars.Vertical
        txtBoxPrint.Size = New Size(888, 373)
        txtBoxPrint.TabIndex = 5
        ' 
        ' btnPreview
        ' 
        btnPreview.AutoSize = True
        btnPreview.Enabled = False
        btnPreview.Location = New Point(24, 61)
        btnPreview.Name = "btnPreview"
        btnPreview.Size = New Size(152, 29)
        btnPreview.TabIndex = 1
        btnPreview.Text = "Vista Previa"
        btnPreview.UseVisualStyleBackColor = True
        ' 
        ' btnPrint
        ' 
        btnPrint.AutoSize = True
        btnPrint.Enabled = False
        btnPrint.Location = New Point(24, 96)
        btnPrint.Name = "btnPrint"
        btnPrint.Size = New Size(152, 29)
        btnPrint.TabIndex = 2
        btnPrint.Text = "Imprimir"
        btnPrint.UseVisualStyleBackColor = True
        ' 
        ' btnExportCSV
        ' 
        btnExportCSV.AutoSize = True
        btnExportCSV.Enabled = False
        btnExportCSV.Location = New Point(24, 131)
        btnExportCSV.Name = "btnExportCSV"
        btnExportCSV.Size = New Size(152, 29)
        btnExportCSV.TabIndex = 3
        btnExportCSV.Text = "Exportar a CSV"
        btnExportCSV.UseVisualStyleBackColor = True
        ' 
        ' panelTextBox
        ' 
        panelTextBox.BorderStyle = BorderStyle.FixedSingle
        panelTextBox.Controls.Add(txtBoxPrint)
        panelTextBox.Location = New Point(207, 26)
        panelTextBox.Name = "panelTextBox"
        panelTextBox.Size = New Size(890, 375)
        panelTextBox.TabIndex = 6
        ' 
        ' mainFrm
        ' 
        AutoScaleDimensions = New SizeF(8F, 19F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1132, 432)
        Controls.Add(panelTextBox)
        Controls.Add(btnExportCSV)
        Controls.Add(btnPrint)
        Controls.Add(btnPreview)
        Controls.Add(btnClose)
        Controls.Add(btnOpen)
        FormBorderStyle = FormBorderStyle.Fixed3D
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        MinimizeBox = False
        Name = "mainFrm"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Lector Archivos Norma 43 CSB v0.1"
        panelTextBox.ResumeLayout(False)
        panelTextBox.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents btnOpen As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents txtBoxPrint As TextBox
    Friend WithEvents btnPreview As Button
    Friend WithEvents btnPrint As Button
    Friend WithEvents btnExportCSV As Button
    Friend WithEvents panelTextBox As Panel

End Class
