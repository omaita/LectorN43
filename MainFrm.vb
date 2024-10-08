Imports System.Drawing.Printing
Imports System.IO
Imports System.Text

Public Class MainFrm

    Dim myRegCodes As New List(Of String)
    Dim myAccounts As New List(Of CabeceraCuentaN43)
    Dim finArchivo As FinalArchivoN43
    Dim currentSaldo As Decimal

    Dim printingFont As Font
    Dim printingText As String
    Dim sbPrint As New StringBuilder
    Dim sbExport As New StringBuilder

    Private Sub BtnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click

        Dim openFileDialog1 As New OpenFileDialog With {
            .InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments,
            .Title = "Abrir Archivo Norma 43",
            .Filter = "Archivos n43 (*.n43)|*.n43|Todos los archivos (*.*)|*.*",
            .FilterIndex = 2,
            .RestoreDirectory = True}

        myRegCodes.Clear()
        myAccounts.Clear()
        finArchivo = Nothing

        If openFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            Try
                Using myStreamReader As New StreamReader(openFileDialog1.FileName)

                    ''Valida los códigos de registro de cada línea del archivo N43
                    While Not myStreamReader.EndOfStream

                        Dim regCode = myStreamReader.ReadLine

                        If regCode.Length <> LONGITUD_LINEA Then
                            MsgBox("Formato de archivo no reconocido", MsgBoxStyle.Critical)
                            Exit Try
                        End If

                        If RegistrosValidos.Contains(regCode.Substring(0, 2)) Then
                            myRegCodes.Add(regCode.Substring(0, 2))
                        Else
                            MsgBox("Formato de archivo no reconocido", MsgBoxStyle.Critical)
                            Exit Try
                        End If

                    End While

                    ''Vuelve al principio del archivo
                    myStreamReader.DiscardBufferedData()
                    myStreamReader.BaseStream.Seek(0, SeekOrigin.Begin)

                    ''Archivo validado: lee los registros.
                    Dim index As Integer
                    While Not myStreamReader.EndOfStream

                        Dim line = myStreamReader.ReadLine

                        If myRegCodes(index) = COD_REGISTRO_CABECERA_CUENTA Then
                            myAccounts.Add(New CabeceraCuentaN43(line))

                        ElseIf myRegCodes(index) = COD_REGISTRO_MOVIMIENTO Then
                            myAccounts.Last.AddMovimiento(line)

                        ElseIf myRegCodes(index) = COD_REGISTRO_CONCEPTO_COMPLEMENTARIO Then
                            myAccounts.Last.Movimientos.Last.AddConceptoComplementario(line)

                        ElseIf myRegCodes(index) = COD_REGISTRO_EQUIVALENCIA_DIVISA Then
                            myAccounts.Last.Movimientos.Last.AddConceptoComplementario(line)

                        ElseIf myRegCodes(index) = COD_REGISTRO_FINAL_CUENTA Then
                            myAccounts.Last.AddFinalCuenta(line)

                        ElseIf myRegCodes(index) = COD_REGISTRO_FIN_FICHERO Then
                            finArchivo = New FinalArchivoN43(line)

                        End If
                        index += 1

                    End While

                End Using
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End If

        If myAccounts.Count > 0 Then
            CreateReport()
        End If

    End Sub

    Private Sub CreateReport()

        txtBoxPrint.Clear()
        sbPrint.Clear()

        'Rellena sb.Print y sb.Export
        For Each cuenta In myAccounts
            PrintCabeceraCuenta(cuenta)
            For Each movimiento In cuenta.Movimientos
                PrintMovimiento(movimiento)
            Next
            PrintFinalCuenta(cuenta.FinCuenta)
        Next
        PrintFinArchivo()

        txtBoxPrint.Text = sbPrint.ToString
        printingText = txtBoxPrint.Text

        btnPreview.Enabled = True
        btnPrint.Enabled = True
        btnExportCSV.Enabled = True

    End Sub

    Private Sub PrintCabeceraCuenta(ByVal account As CabeceraCuentaN43)

        ''Para crear un archivo UTF8 con BOM hay que añadir estos caracteres al principio del archivo.
        Dim saveToUTFBOM As String = Encoding.UTF8.GetString({&HEF, &HBB, &HBF})

        sbPrint.Append("TITULAR: " & account.NombreCliente & vbCrLf)
        sbPrint.Append("NUMERO DE CUENTA: " & account.ClaveEntidad & " " & account.ClaveOficina & " " & account.NumeroCuenta & vbCrLf)
        sbPrint.Append("SALDO INICIAL: " & If(account.ClaveDebeHaber = "1", Format(-account.SaldoInicial, "N"), Format(account.SaldoInicial, "N")) _
            & "    " & "DIVISA: " & account.Divisa & vbCrLf)
        sbPrint.Append("Movimientos de cuenta desde " & Format(account.FechaInicial, "dd/MM/yyyy") & " hasta " & Format(account.FechaFinal, "dd/MM/yyyy") & vbCrLf & vbCrLf)
        sbPrint.Append("Fecha Operación".PadRight(20) & "Fecha Valor".PadRight(36) & "Importe".PadLeft(14) & "Saldo".PadLeft(14) & vbCrLf)
        sbPrint.Append(Strings.StrDup(84, "-"c) & vbCrLf)

        ''1 = Saldo deudor (negativo), 2 = Saldo Acreedor (positivo)
        If account.ClaveDebeHaber = "1" Then
            currentSaldo = -account.SaldoInicial
        Else
            currentSaldo = account.SaldoInicial
        End If

        sbExport.Append(saveToUTFBOM)
        sbExport.Append(";;" & "TITULAR: " & account.NombreCliente & ";;;" & vbCrLf)
        sbExport.Append(";;" & "NUMERO DE CUENTA: " & account.ClaveEntidad & " " & account.ClaveOficina & " " & account.NumeroCuenta & ";;;" & vbCrLf)
        sbExport.Append(";;" & "SALDO INICIAL:;;" & If(account.ClaveDebeHaber = "1", -account.SaldoInicial, account.SaldoInicial) & ";" & account.Divisa & ";" & vbCrLf)
        sbExport.Append(";;" & "Movimientos cuenta desde " & account.FechaInicial & " hasta " & account.FechaFinal & ";;;" & vbCrLf)
        sbExport.Append(";;;;;" & vbCrLf)
        sbExport.Append("FECHA OPERACIÓN;FECHA VALOR;CONCEPTO;IMPORTE;SALDO;" & vbCrLf)

    End Sub

    Private Sub PrintMovimiento(ByVal movimiento As MovimientoN43)

        If movimiento.ClaveDebeHaber = "1" Then
            currentSaldo -= movimiento.Importe
        Else
            currentSaldo += movimiento.Importe
        End If

        sbPrint.Append(Format(movimiento.FechaOperacion, "dd/MM/yyyy").PadRight(20) _
                & Format(movimiento.FechaValor, "dd/MM/yyyy").PadRight(36) _
                & If(movimiento.ClaveDebeHaber = "1", Format(-movimiento.Importe, "N").PadLeft(14), Format(movimiento.Importe, "N").PadLeft(14)) _
                & Format(currentSaldo, "N").PadLeft(14) & vbCrLf)
        sbPrint.Append(movimiento.ConceptoComun.PadRight(60) & vbCrLf)
        For Each concepto In movimiento.ConceptosComplementarios
            sbPrint.Append(concepto.Concepto1.PadRight(38) & concepto.Concepto2.PadRight(38) & vbCrLf)
        Next
        If movimiento.EquivalenciaDivisa IsNot Nothing Then
            sbPrint.Append("Equivalencia en " & movimiento.EquivalenciaDivisa.Divisa.PadRight(24) & Format(movimiento.EquivalenciaDivisa.Importe, "N").PadLeft(12) & vbCrLf)
        End If
        sbPrint.Append(Strings.StrDup(84, "-"c) & vbCrLf)

        sbExport.Append(movimiento.FechaOperacion & ";" & movimiento.FechaValor & ";" & movimiento.ConceptoComun & ";" & If(movimiento.ClaveDebeHaber = "1", -movimiento.Importe, movimiento.Importe) & ";" & currentSaldo & ";" & vbCrLf)
        For Each concepto In movimiento.ConceptosComplementarios
            sbExport.Append(";;" & concepto.Concepto1.Trim & concepto.Concepto2.Trim & ";;;" & vbCrLf)
        Next
        If movimiento.EquivalenciaDivisa IsNot Nothing Then
            sbExport.Append("Equivalencia en;" & movimiento.EquivalenciaDivisa.Divisa & ";" & movimiento.EquivalenciaDivisa.Importe & ";;;" & vbCrLf)
        End If

    End Sub

    Private Sub PrintFinalCuenta(ByVal finCuenta As FinalCuentaN43)

        sbPrint.Append("Número Apuntes Debe  :" & Format(finCuenta.NumeroApuntesDebe, "G").PadLeft(16) & vbCrLf)
        sbPrint.Append("Total Importes Debe  :" & Format(finCuenta.TotalImportesDebe, "N").PadLeft(16) & vbCrLf)
        sbPrint.Append("Número Apuntes Haber :" & Format(finCuenta.NumeroApuntesHaber, "G").PadLeft(16) & vbCrLf)
        sbPrint.Append("Total Importes Haber :" & Format(finCuenta.TotalImportesHaber, "N").PadLeft(16) & vbCrLf)
        sbPrint.Append("Saldo Final          :" _
                  & If(finCuenta.CodigoSaldoFinal = "1", Format(-finCuenta.SaldoFinal, "N").PadLeft(16), Format(finCuenta.SaldoFinal, "N").PadLeft(16)) _
                  & " " & finCuenta.Divisa & vbCrLf & vbCrLf)

        sbExport.Append(";;" & "SALDO FINAL:;;" & If(finCuenta.CodigoSaldoFinal = "1", -finCuenta.SaldoFinal, finCuenta.SaldoFinal) & ";" & finCuenta.Divisa & ";" & vbCrLf & vbCrLf)
        sbExport.Append(";;" & "Número Apuntes Debe:;" & finCuenta.NumeroApuntesDebe & ";;" & vbCrLf)
        sbExport.Append(";;" & "Total Importes Debe:;" & finCuenta.TotalImportesDebe & ";;" & vbCrLf)
        sbExport.Append(";;" & "Número Apuntes Haber:;" & finCuenta.NumeroApuntesHaber & ";;" & vbCrLf)
        sbExport.Append(";;" & "Total Importes Haber:;" & finCuenta.TotalImportesHaber & ";;" & vbCrLf & vbCrLf)

    End Sub

    Private Sub PrintFinArchivo()

        sbPrint.Append(Strings.StrDup(84, "-"c) & vbCrLf)
        sbPrint.Append("Número de Registros: " & finArchivo.NumeroRegistros & vbCrLf)
        sbPrint.Append(Strings.StrDup(84, "-"c) & vbCrLf)
        sbPrint.Append("******End of report******")

        sbExport.Append(";;" & "Número de Registros:;" & finArchivo.NumeroRegistros & ";;")

    End Sub

    Private Sub BtnPreview_Click(sender As Object, e As EventArgs) Handles btnPreview.Click

        Dim myPrintDocument As New PrintDocument
        Dim myPrintPreview As New PrintPreviewDialog

        Dim margins As New Margins(50, 50, 50, 50)
        myPrintDocument.DefaultPageSettings.Margins = margins

        ''Fuente por defecto para imprimir solo queda bien con fuentes monoespaciadas, no con proprocionales
        printingFont = New Font("Courier New", 10)

        ''Agrega un controlador de eventos para el evento PrintPage del objeto myPrintDocument
        AddHandler myPrintDocument.PrintPage, AddressOf PrintDocument_PrintPage

        ''Imprime con printpreview
        myPrintPreview.Document = myPrintDocument
        myPrintPreview.ShowDialog()

    End Sub

    Private Sub BtnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click

        Dim myPrintDocument As New PrintDocument
        Dim myPrintDialog As New PrintDialog

        Dim margins As New Margins(50, 50, 50, 50)
        myPrintDocument.DefaultPageSettings.Margins = margins

        ''Fuente por defecto para imprimir solo queda bien con fuentes monoespaciadas, no con proprocionales
        printingFont = New Font("Courier New", 10)

        ''Agrega un controlador de eventos para el evento PrintPage del objeto myPrintDocument
        AddHandler myPrintDocument.PrintPage, AddressOf PrintDocument_PrintPage

        ''Imprime con selección de impresora
        myPrintDialog.Document = myPrintDocument
        If myPrintDialog.ShowDialog = DialogResult.OK Then
            myPrintDocument.Print()
        End If

    End Sub


    Private Sub PrintDocument_PrintPage(ByVal sender As Object, ByVal e As PrintPageEventArgs)

        Dim charactersOnPage As Integer = 0
        Dim linesPerPage As Integer = 0

        ' Establece el valor de charactersOnPage en la cantidad de caracteres
        ' de stringToPrint que caben dentro de los límites de la página.
        e.Graphics.MeasureString(printingText, printingFont, e.MarginBounds.Size, StringFormat.GenericTypographic, charactersOnPage, linesPerPage)

        ' Dibuja la cadena dentro de los límites de la página
        e.Graphics.DrawString(printingText, printingFont, Brushes.Black, e.MarginBounds, StringFormat.GenericTypographic)

        ' Elimina la parte de la cadena que se ha impreso.
        printingText = printingText.Substring(charactersOnPage)

        ' Comprueba si se deben imprimir más páginas.
        If printingText.Length > 0 Then
            e.HasMorePages = True
        Else
            e.HasMorePages = False
            printingText = txtBoxPrint.Text
        End If

    End Sub

    Private Sub BtnExportCSV_Click(sender As Object, e As EventArgs) Handles btnExportCSV.Click

        Dim mySaveFileDialog As New SaveFileDialog With {
            .InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments,
            .Title = "Exportar a archivo CSV",
            .Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*",
            .FilterIndex = 1,
            .RestoreDirectory = True
        }

        If mySaveFileDialog.ShowDialog() = DialogResult.OK Then
            Try
                File.WriteAllText(mySaveFileDialog.FileName, sbExport.ToString)
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End If

    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click

        Me.Close()

    End Sub

End Class


