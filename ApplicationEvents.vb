Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        Private Sub MyApplication_Startup(sender As Object, e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            AppLogger.Info("Application startup")
            AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException
            AddHandler System.Windows.Forms.Application.ThreadException, AddressOf Application_ThreadException
        End Sub

        Private Sub MyApplication_UnhandledException(sender As Object, e As ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException
            AppLogger.LogError("Unhandled exception", e.Exception)
            e.ExitApplication = True
        End Sub

        Private Sub Application_ThreadException(sender As Object, e As Threading.ThreadExceptionEventArgs)
            AppLogger.LogError("Thread exception", e.Exception)
        End Sub

        Private Sub CurrentDomain_UnhandledException(sender As Object, e As UnhandledExceptionEventArgs)
            Dim ex As Exception = TryCast(e.ExceptionObject, Exception)
            AppLogger.LogError("AppDomain unhandled exception", ex)
        End Sub
    End Class
End Namespace
