<?php
include_once('../connects.php');

header('Content-Type: application/json; charset=utf-8');

$paramsSet = isset($_GET['user_id']);

$attendanceJson = array();

if ($paramsSet) {
    $id = (int)$_GET['user_id'];
    $query = "SELECT att.user_id as user_id, usr.first_name as first_name, usr.last_name as last_name, CONVERT_TZ(att.date_time, '+00:00', '+08:00') as date_time FROM attendance_log att INNER JOIN user_accounts usr ON att.user_id = usr.user_id WHERE att.user_id = ? ORDER BY att.date_time DESC";
    
    $statement = $con->prepare($query);
    $statement->bind_param('i', $id);
    $statement->execute();

    $result = $statement->get_result();
    $logs = $result->fetch_all();
    
    foreach ($logs as $log) {
        $logJson = array('user_id'=>$log[0], 'first_name'=>$log[1], 'last_name'=>$log[2], 'date_time'=>$log[3]);
        array_push($attendanceJson, $logJson);
    }
}
else {
    $query = "SELECT att.user_id as user_id, usr.first_name as first_name, usr.last_name as last_name, CONVERT_TZ(att.date_time, '+00:00', '+08:00') as date_time FROM attendance_log att INNER JOIN user_accounts usr ON att.user_id = usr.user_id ORDER BY att.date_time DESC";
    $statement = $con->prepare($query);
    $statement->execute();

    $result = $statement->get_result();
    $logs = $result->fetch_all();
    
    foreach ($logs as $log) {
        $logJson = array('user_id'=>$log[0], 'first_name'=>$log[1], 'last_name'=>$log[2], 'date_time'=>$log[3]);
        array_push($attendanceJson, $logJson);
    }
}

echo json_encode($attendanceJson);
?>