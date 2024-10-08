Public Module ModuleN43

    Public Const LONGITUD_LINEA As Integer = 80

    Public Const COD_REGISTRO_CABECERA_CUENTA As String = "11"
    Public Const COD_REGISTRO_MOVIMIENTO As String = "22"
    Public Const COD_REGISTRO_CONCEPTO_COMPLEMENTARIO As String = "23"
    Public Const COD_REGISTRO_EQUIVALENCIA_DIVISA As String = "24"
    Public Const COD_REGISTRO_FINAL_CUENTA As String = "33"
    Public Const COD_REGISTRO_FIN_FICHERO As String = "88"

    Public RegistrosValidos As New List(Of String) From {
        {COD_REGISTRO_CABECERA_CUENTA},
        {COD_REGISTRO_MOVIMIENTO},
        {COD_REGISTRO_CONCEPTO_COMPLEMENTARIO},
        {COD_REGISTRO_EQUIVALENCIA_DIVISA},
        {COD_REGISTRO_FINAL_CUENTA},
        {COD_REGISTRO_FIN_FICHERO}}

    Public Conceptos As New Dictionary(Of String, String) From {
        {"01", "TALONES - REINTEGROS"},
        {"02", "ABONARÉS - ENTREGAS - INGRESOS"},
        {"03", "DOMICILIADOS - RECIBOS - LETRAS - PAGOS POR SU CTA."},
        {"04", "GIROS - TRANSFERENCIAS - TRASPASOS - CHEQUES"},
        {"05", "AMORTIZACIONES PRÉSTAMOS, CRÉDITOS, ETC."},
        {"06", "REMESAS EFECTOS"},
        {"07", "SUSCRIPCIONES - DIV. PASIVOS - CANJES."},
        {"08", "DIV. CUPONES - PRIMA JUNTA - AMORTIZACIONES"},
        {"09", "OPERACIONES DE BOLSA Y/O COMPRA /VENTA VALORES"},
        {"10", "CHEQUES GASOLINA"},
        {"11", "CAJERO AUTOMÁTICO"},
        {"12", "TARJETAS DE CRÉDITO - TARJETAS DÉBITO"},
        {"13", "OPERACIONES EXTRANJERO"},
        {"14", "DEVOLUCIONES E IMPAGADOS"},
        {"15", "NÓMINAS - SEGUROS SOCIALES"},
        {"16", "TIMBRES - CORRETAJE - PÓLIZA"},
        {"17", "INTERESES - COMISIONES – CUSTODIA - GASTOS E IMPUESTOS"},
        {"98", "ANULACIONES - CORRECCIONES ASIENTO"},
        {"99", "VARIOS"}}

    Public Divisas As New Dictionary(Of String, String) From {
        {"36", "Dólar australiano"},
        {"124", "Dólar canadiense"},
        {"208", "Corona Danesa"},
        {"392", "Yen japonés"},
        {"554", "Dólar neozelandés"},
        {"578", "Corona noruega"},
        {"752", "Corona sueca"},
        {"756", "Franco suizo"},
        {"826", "Libra esterlina"},
        {"840", "Dólar USA"},
        {"978", "Euro"}}

    ''' <summary>
    ''' Registro de cabecera de cuenta
    ''' </summary>
    Public Class CabeceraCuentaN43

        Private _lineRead As String

        Private _CodigoRegistro As String
        Private _ClaveEntidad As String
        Private _ClaveOficina As String
        Private _NumeroCuenta As String
        Private _FI_Year As String
        Private _FI_Month As String
        Private _FI_Day As String
        Private _FF_Year As String
        Private _FF_Month As String
        Private _FF_Day As String
        Private _ClaveDebeHaber As String
        Private _SaldoInicial As String
        Private _ClaveDivisa As String
        Private _ModalidadInfo As String
        Private _NombreCliente As String
        Private _Libre As String

        Private _movimientos As New List(Of MovimientoN43)
        Private _finalCuenta As FinalCuentaN43

        Public Sub New(line As String)

            If Not String.IsNullOrEmpty(line) Then

                _lineRead = line

                _CodigoRegistro = _lineRead.Substring(0, 2)
                If _CodigoRegistro <> COD_REGISTRO_CABECERA_CUENTA Then Exit Sub

                _ClaveEntidad = _lineRead.Substring(2, 4)
                _ClaveOficina = _lineRead.Substring(6, 4)
                _NumeroCuenta = _lineRead.Substring(10, 10)

                _FI_Year = String.Concat("20", _lineRead.AsSpan(20, 2))
                _FI_Month = _lineRead.Substring(22, 2)
                _FI_Day = _lineRead.Substring(24, 2)

                _FF_Year = String.Concat("20", _lineRead.AsSpan(26, 2))
                _FF_Month = _lineRead.Substring(28, 2)
                _FF_Day = _lineRead.Substring(30, 2)

                _ClaveDebeHaber = _lineRead.Substring(32, 1)
                _SaldoInicial = _lineRead.Substring(33, 14)
                _ClaveDivisa = _lineRead.Substring(47, 3)
                _ModalidadInfo = _lineRead.Substring(50, 1)
                _NombreCliente = _lineRead.Substring(51, 26)
                _Libre = _lineRead.Substring(77, 3)

            End If

        End Sub

        Public ReadOnly Property CodigoRegistro As String
            Get
                Return _CodigoRegistro
            End Get
        End Property

        Public ReadOnly Property ClaveEntidad As String
            Get
                Return _ClaveEntidad
            End Get
        End Property

        Public ReadOnly Property ClaveOficina As String
            Get
                Return _ClaveOficina
            End Get
        End Property

        Public ReadOnly Property NumeroCuenta As String
            Get
                Return _NumeroCuenta
            End Get
        End Property

        Public ReadOnly Property FechaInicial As Date
            Get
                If _FI_Year IsNot Nothing And _FI_Month IsNot Nothing And _FI_Day IsNot Nothing Then
                    Return New Date(Integer.Parse(_FI_Year), Integer.Parse(_FI_Month), Integer.Parse(_FI_Day))
                Else
                    Return Today
                End If
            End Get
        End Property

        Public ReadOnly Property FechaFinal As Date
            Get
                If _FF_Year IsNot Nothing And _FF_Month IsNot Nothing And _FF_Day IsNot Nothing Then
                    Return New Date(Integer.Parse(_FF_Year), Integer.Parse(_FF_Month), Integer.Parse(_FF_Day))
                Else
                    Return Today
                End If
            End Get
        End Property

        ''1=Deudor, 2=Acreedor
        Public ReadOnly Property ClaveDebeHaber As String
            Get
                Return _ClaveDebeHaber
            End Get
        End Property

        Public ReadOnly Property SaldoInicial As Decimal
            Get
                If _SaldoInicial Is Nothing Then Return 0D
                Dim number As Decimal
                Dim success As Boolean = Decimal.TryParse((_SaldoInicial.TrimStart("0"c)), number)
                Return If(success, number / 100D, 0D)
            End Get
        End Property

        Public ReadOnly Property Divisa As String
            Get
                If _ClaveDivisa Is Nothing Then Return String.Empty
                Dim value As String = Nothing
                Return If(Divisas.TryGetValue(_ClaveDivisa, value), value, String.Empty)
            End Get
        End Property

        Public ReadOnly Property ModalidadInfo As String
            Get
                Return _ModalidadInfo
            End Get
        End Property

        Public ReadOnly Property NombreCliente As String
            Get
                Return _NombreCliente
            End Get
        End Property

        Public ReadOnly Property Libre As String
            Get
                Return _Libre
            End Get
        End Property

        Public ReadOnly Property Movimientos As List(Of MovimientoN43)
            Get
                Return _movimientos
            End Get
        End Property

        Public ReadOnly Property FinCuenta As FinalCuentaN43
            Get
                Return _finalCuenta
            End Get
        End Property

        Public Sub AddMovimiento(ByVal line As String)

            _movimientos.Add(New MovimientoN43(line))

        End Sub

        Public Sub AddFinalCuenta(ByVal line As String)

            _finalCuenta = New FinalCuentaN43(line)

        End Sub

    End Class


    ''' <summary>
    ''' Registro principal de movimientos (obligatorio)
    ''' </summary>
    Public Class MovimientoN43

        Private _lineRead As String

        Private _CodigoRegistro As String
        Private _Libre As String
        Private _ClaveOficinaOrigen As String
        Private _FO_Year As String
        Private _FO_Month As String
        Private _FO_Day As String
        Private _FV_Year As String
        Private _FV_Month As String
        Private _FV_Day As String
        Private _ConceptoComun As String
        Private _ConceptoPropio As String
        Private _ClaveDebeHaber As String
        Private _ImporteApunte As String
        Private _Documento As String
        Private _Referencia1 As String
        Private _Referencia2 As String

        Private _ConceptosComplementarios As New List(Of ConceptoComplementarioN43)
        Private _EquivalenciaDivisa As EquivalenciaDivisaN43

        Public Sub New(line As String)

            If Not String.IsNullOrEmpty(line) Then

                _lineRead = line

                _CodigoRegistro = _lineRead.Substring(0, 2)
                If _CodigoRegistro <> COD_REGISTRO_MOVIMIENTO Then Exit Sub

                _Libre = _lineRead.Substring(2, 4)
                _ClaveOficinaOrigen = _lineRead.Substring(6, 4)

                _FO_Year = String.Concat("20", _lineRead.AsSpan(10, 2))
                _FO_Month = _lineRead.Substring(12, 2)
                _FO_Day = _lineRead.Substring(14, 2)

                _FV_Year = String.Concat("20", _lineRead.AsSpan(16, 2))
                _FV_Month = _lineRead.Substring(18, 2)
                _FV_Day = _lineRead.Substring(20, 2)

                _ConceptoComun = _lineRead.Substring(22, 2)
                _ConceptoPropio = _lineRead.Substring(24, 3)

                _ClaveDebeHaber = _lineRead.Substring(27, 1)
                _ImporteApunte = _lineRead.Substring(28, 14)
                _Documento = _lineRead.Substring(42, 10)
                _Referencia1 = _lineRead.Substring(52, 12)
                _Referencia2 = _lineRead.Substring(64, 16)

            End If

        End Sub

        Public ReadOnly Property CodigoRegistro As String
            Get
                Return _CodigoRegistro
            End Get
        End Property

        Public ReadOnly Property Libre As String
            Get
                Return _Libre
            End Get
        End Property

        Public ReadOnly Property ClaveOficinaOrigen As String
            Get
                Return _ClaveOficinaOrigen
            End Get
        End Property

        Public ReadOnly Property FechaOperacion As Date
            Get
                If _FO_Year IsNot Nothing And _FO_Month IsNot Nothing And _FO_Day IsNot Nothing Then
                    Return New Date(Integer.Parse(_FO_Year), Integer.Parse(_FO_Month), Integer.Parse(_FO_Day))
                Else
                    Return Today
                End If
            End Get
        End Property

        Public ReadOnly Property FechaValor As Date
            Get
                If _FV_Year IsNot Nothing And _FV_Month IsNot Nothing And _FV_Day IsNot Nothing Then
                    Return New Date(Integer.Parse(_FV_Year), Integer.Parse(_FV_Month), Integer.Parse(_FV_Day))
                Else
                    Return Today
                End If
            End Get
        End Property

        Public ReadOnly Property ConceptoComun As String
            Get
                If _ConceptoComun Is Nothing Then Return String.Empty
                Dim value As String = Nothing
                Return If(Conceptos.TryGetValue(_ConceptoComun, value), value, String.Empty)
            End Get
        End Property

        Public ReadOnly Property ConceptoPropio As String
            Get
                Return _ConceptoPropio
            End Get
        End Property

        ''1=Debe, 2=Haber
        Public ReadOnly Property ClaveDebeHaber As String
            Get
                Return _ClaveDebeHaber
            End Get
        End Property

        Public ReadOnly Property Importe As Decimal
            Get
                If _ImporteApunte Is Nothing Then Return 0D
                Dim number As Decimal
                Dim success As Boolean = Decimal.TryParse((_ImporteApunte.TrimStart("0"c)), number)
                Return If(success, number / 100D, 0D)
            End Get
        End Property

        Public ReadOnly Property Documento As String
            Get
                Return _Documento
            End Get
        End Property

        Public ReadOnly Property Referencia1 As String
            Get
                Return _Referencia1
            End Get
        End Property

        Public ReadOnly Property Referencia2 As String
            Get
                Return _Referencia2
            End Get
        End Property

        Public ReadOnly Property ConceptosComplementarios As List(Of ConceptoComplementarioN43)
            Get
                Return _ConceptosComplementarios
            End Get
        End Property

        Public ReadOnly Property EquivalenciaDivisa As EquivalenciaDivisaN43
            Get
                Return _EquivalenciaDivisa
            End Get
        End Property

        Public Sub AddConceptoComplementario(ByVal line As String)
            _ConceptosComplementarios.Add(New ConceptoComplementarioN43(line))
        End Sub

        Public Sub AddEquivalenciaDivisa(ByVal line As String)
            _EquivalenciaDivisa = New EquivalenciaDivisaN43(line)
        End Sub

    End Class


    ''' <summary>
    ''' Registros complementarios de concepto. Primero a quinto opcionales
    ''' </summary>
    Public Class ConceptoComplementarioN43

        Private _lineRead As String

        Private _CodigoRegistro As String
        Private _CodigoDato As String
        Private _Concepto1 As String
        Private _Concepto2 As String

        Public Sub New(line As String)

            If Not String.IsNullOrEmpty(line) Then

                _lineRead = line

                _CodigoRegistro = _lineRead.Substring(0, 2)
                If _CodigoRegistro <> COD_REGISTRO_CONCEPTO_COMPLEMENTARIO Then Exit Sub

                _CodigoDato = _lineRead.Substring(2, 2)
                _Concepto1 = _lineRead.Substring(4, 38)
                _Concepto2 = _lineRead.Substring(42, 38)

            End If

        End Sub

        Public ReadOnly Property CodigoRegistro As String
            Get
                Return _CodigoRegistro
            End Get
        End Property

        Public ReadOnly Property CodigoDato As String
            Get
                Return _CodigoDato
            End Get
        End Property

        Public ReadOnly Property Concepto1 As String
            Get
                Return _Concepto1
            End Get
        End Property

        Public ReadOnly Property Concepto2 As String
            Get
                Return _Concepto2
            End Get
        End Property

    End Class


    ''' <summary>
    ''' Registro complementario de información de equivalencia de importe del apunte (Opcional)
    ''' 
    ''' Código de registro: dos posiciones.
    ''' - 24
    ''' Código de dato: dos posiciones.
    ''' - 01
    ''' Clave de divisa origen del movimiento: tres posiciones.
    ''' - Según tabla de divisas y claves código ISO (ver anexo 2).
    ''' Importe: catorce posiciones
    ''' - Importe del apunte en la clave de divisa de origen, relleno con ceros a la izquierda, si es necesario.
    ''' - 12 posiciones para enteros y 2 para decimales, sin reflejar la coma.
    ''' Libre: cincuenta y nueve posiciones
    ''' - Relleno a espacios
    ''' 
    ''' Este registro, sin valor contable, únicamente figurará cuando la moneda origen de la operación no
    ''' sea coincidente con el tipo de moneda de la cuenta.
    ''' </summary>
    Public Class EquivalenciaDivisaN43

        Private _lineRead As String

        Private _CodigoRegistro As String
        Private _CodigoDato As String
        Private _ClaveDivisa As String
        Private _ImporteApunte As String
        Private _Libre As String

        Public Sub New(line As String)

            If Not String.IsNullOrEmpty(line) Then

                _lineRead = line

                _CodigoRegistro = _lineRead.Substring(0, 2)
                If _CodigoRegistro <> COD_REGISTRO_EQUIVALENCIA_DIVISA Then Exit Sub

                _ClaveDivisa = _lineRead.Substring(4, 3)
                _ImporteApunte = _lineRead.Substring(7, 14)
                _Libre = _lineRead.Substring(21, 59)

            End If

        End Sub

        Public ReadOnly Property CodigoRegistro As String
            Get
                Return _CodigoRegistro
            End Get
        End Property

        Public ReadOnly Property CodigoDato As String
            Get
                Return _CodigoDato
            End Get
        End Property

        Public ReadOnly Property Divisa As String
            Get
                If _ClaveDivisa Is Nothing Then Return String.Empty
                Dim value As String = Nothing
                Return If(Divisas.TryGetValue(_ClaveDivisa, value), value, String.Empty)
            End Get
        End Property

        Public ReadOnly Property Importe As Decimal
            Get
                If _ImporteApunte Is Nothing Then Return 0D
                Dim number As Decimal
                Dim success As Boolean = Decimal.TryParse((_ImporteApunte.TrimStart("0"c)), number)
                Return If(success, number / 100D, 0D)
            End Get
        End Property

        Public ReadOnly Property Libre As String
            Get
                Return _Libre
            End Get
        End Property

    End Class


    ''' <summary>
    ''' Registro final de la cuenta
    ''' </summary>
    Public Class FinalCuentaN43

        Private _lineRead As String

        Private _CodigoRegistro As String
        Private _ClaveEntidad As String
        Private _ClaveOficina As String
        Private _NumeroCuenta As String
        Private _NumeroApuntesDebe As String
        Private _TotalImportesDebe As String
        Private _NumeroApuntesHaber As String
        Private _TotalImportesHaber As String
        Private _CodigoSaldoFinal As String
        Private _SaldoFinal As String
        Private _ClaveDivisa As String
        Private _Libre As String

        Public Sub New(line As String)

            If Not String.IsNullOrEmpty(line) Then

                _lineRead = line

                _CodigoRegistro = _lineRead.Substring(0, 2)
                If _CodigoRegistro <> COD_REGISTRO_FINAL_CUENTA Then Exit Sub

                _ClaveEntidad = _lineRead.Substring(2, 4)
                _ClaveOficina = _lineRead.Substring(6, 4)
                _NumeroCuenta = _lineRead.Substring(10, 10)

                _NumeroApuntesDebe = _lineRead.Substring(20, 5)
                _TotalImportesDebe = _lineRead.Substring(25, 14)

                _NumeroApuntesHaber = _lineRead.Substring(39, 5)
                _TotalImportesHaber = _lineRead.Substring(44, 14)

                _CodigoSaldoFinal = _lineRead.Substring(58, 1)
                _SaldoFinal = _lineRead.Substring(59, 14)
                _ClaveDivisa = _lineRead.Substring(73, 3)
                _Libre = _lineRead.Substring(76, 4)

            End If

        End Sub

        Public ReadOnly Property CodigoRegistro As String
            Get
                Return _CodigoRegistro
            End Get
        End Property

        Public ReadOnly Property ClaveEntidad As String
            Get
                Return _ClaveEntidad
            End Get
        End Property

        Public ReadOnly Property ClaveOficina As String
            Get
                Return _ClaveOficina
            End Get
        End Property

        Public ReadOnly Property NumeroCuenta As String
            Get
                Return _NumeroCuenta
            End Get
        End Property

        Public ReadOnly Property NumeroApuntesDebe As Integer
            Get
                If _NumeroApuntesDebe Is Nothing Then Return 0
                Dim number As Integer
                Dim success As Boolean = Integer.TryParse((_NumeroApuntesDebe.TrimStart("0"c)), number)
                Return If(success, number, 0)
            End Get
        End Property

        Public ReadOnly Property TotalImportesDebe As Decimal
            Get
                If _TotalImportesDebe Is Nothing Then Return 0D
                Dim number As Decimal
                Dim success As Boolean = Decimal.TryParse((_TotalImportesDebe.TrimStart("0"c)), number)
                Return If(success, number / 100D, 0D)
            End Get
        End Property

        Public ReadOnly Property NumeroApuntesHaber As Integer
            Get
                If _NumeroApuntesHaber Is Nothing Then Return 0
                Dim number As Integer
                Dim success As Boolean = Integer.TryParse((_NumeroApuntesHaber.TrimStart("0"c)), number)
                Return If(success, number, 0)
            End Get
        End Property


        Public ReadOnly Property TotalImportesHaber As Decimal
            Get
                If _TotalImportesHaber Is Nothing Then Return 0D
                Dim number As Decimal
                Dim success As Boolean = Decimal.TryParse((_TotalImportesHaber.TrimStart("0"c)), number)
                Return If(success, number / 100D, 0D)
            End Get
        End Property

        ''1=Deudor, 2=Acreedor
        Public ReadOnly Property CodigoSaldoFinal As String
            Get
                Return _CodigoSaldoFinal
            End Get
        End Property

        Public ReadOnly Property SaldoFinal As Decimal
            Get
                If _SaldoFinal Is Nothing Then Return 0D
                Dim number As Decimal
                Dim success As Boolean = Decimal.TryParse((_SaldoFinal.TrimStart("0"c)), number)
                Return If(success, number / 100D, 0D)
            End Get
        End Property

        Public ReadOnly Property Divisa As String
            Get
                If _ClaveDivisa Is Nothing Then Return String.Empty
                Dim value As String = Nothing
                Return If(Divisas.TryGetValue(_ClaveDivisa, value), value, String.Empty)
            End Get
        End Property

        Public ReadOnly Property Libre As String
            Get
                Return _Libre
            End Get
        End Property

    End Class


    ''' <summary>
    ''' Registro de fin de fichero
    ''' </summary>
    Public Class FinalArchivoN43

        Private _lineRead As String

        Private _CodigoRegistro As String
        Private _Nueves As String
        Private _NumeroRegistros As String
        Private _Libre As String

        Public Sub New(line As String)

            If Not String.IsNullOrEmpty(line) Then

                _lineRead = line

                _CodigoRegistro = _lineRead.Substring(0, 2)
                If _CodigoRegistro <> COD_REGISTRO_FIN_FICHERO Then Exit Sub

                _Nueves = _lineRead.Substring(2, 18)
                _NumeroRegistros = _lineRead.Substring(20, 6)
                _Libre = _lineRead.Substring(26, 54)

            End If

        End Sub

        Private ReadOnly Property CodigoRegistro As String
            Get
                Return _CodigoRegistro
            End Get
        End Property

        Public ReadOnly Property Nueves As String
            Get
                Return _Nueves
            End Get
        End Property

        Public ReadOnly Property NumeroRegistros As Integer
            Get
                If _NumeroRegistros Is Nothing Then Return 0
                Dim number As Integer
                Dim success As Boolean = Integer.TryParse((_NumeroRegistros.TrimStart("0"c)), number)
                Return If(success, number, 0)
            End Get
        End Property

        Public ReadOnly Property Libre As String
            Get
                Return _Libre
            End Get
        End Property

    End Class



    Public Function CalculateDC(ByVal ClaveEntidad As String, ByVal ClaveOficina As String, ByVal NumeroCuenta As String) As String

        Dim dcOne(8) As Integer
        Dim dcTwo(10) As Integer

        dcOne(0) = Integer.Parse(ClaveEntidad(0)) * 4
        dcOne(1) = Integer.Parse(ClaveEntidad(1)) * 8
        dcOne(2) = Integer.Parse(ClaveEntidad(2)) * 5
        dcOne(3) = Integer.Parse(ClaveEntidad(3)) * 10

        dcOne(4) = Integer.Parse(ClaveOficina(0)) * 9
        dcOne(5) = Integer.Parse(ClaveOficina(1)) * 7
        dcOne(6) = Integer.Parse(ClaveOficina(2)) * 3
        dcOne(7) = Integer.Parse(ClaveOficina(3)) * 6

        Dim digitOne = 11 - dcOne.Sum Mod 11
        If digitOne > 9 Then
            digitOne = 1 - (digitOne Mod 10)
        End If

        dcTwo(0) = Integer.Parse(NumeroCuenta(0)) * 1
        dcTwo(1) = Integer.Parse(NumeroCuenta(1)) * 2
        dcTwo(2) = Integer.Parse(NumeroCuenta(2)) * 4
        dcTwo(3) = Integer.Parse(NumeroCuenta(3)) * 8
        dcTwo(4) = Integer.Parse(NumeroCuenta(4)) * 5
        dcTwo(5) = Integer.Parse(NumeroCuenta(5)) * 10
        dcTwo(6) = Integer.Parse(NumeroCuenta(6)) * 9
        dcTwo(7) = Integer.Parse(NumeroCuenta(7)) * 7
        dcTwo(8) = Integer.Parse(NumeroCuenta(8)) * 3
        dcTwo(9) = Integer.Parse(NumeroCuenta(9)) * 6

        Dim digitTwo = 11 - dcTwo.Sum Mod 11
        If digitTwo > 9 Then
            digitTwo = 1 - (digitTwo Mod 10)
        End If

        Return CStr(digitOne & digitTwo)

    End Function

End Module
