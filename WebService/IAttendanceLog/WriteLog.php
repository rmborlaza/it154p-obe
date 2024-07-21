<?php 
include_once('../connects.php');

$paramsSet = isset($_POST['serial_no']);

if (!$paramsSet)
{
    http_response_code(401);
    echo 'ERROR';
    die();
}

$serial = $_POST['serial_no'];

try {
    $query = "INSERT INTO attendance_log (user_id) SELECT user_id FROM user_accounts WHERE card_serial_no=?";
    $statement = $con->prepare($query);
    $statement->bind_param('s', $serial);
    $statement->execute();
    $affectedRows = $statement->affected_rows;
    
    if ($affectedRows > 0) {
        echo 'OK';
    }
    else {
        echo 'UNIDENTIFIED';
    }
}
catch (Exception $ex) {
    http_response_code(500);
    echo 'ERROR';
}

?>