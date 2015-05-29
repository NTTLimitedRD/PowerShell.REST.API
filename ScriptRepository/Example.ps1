param ($message)
start-job -scriptblock { $args[0].GetType().FullName } -argumentlist { echo $message }