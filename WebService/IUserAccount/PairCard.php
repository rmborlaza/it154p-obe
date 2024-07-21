<?php
include_once('../connects.php');

$paramsSet = isset($_POST['user_id']) && isset($_POST['serial_no']);

if (!$paramsSet)
{
    http_response_code(400);
    echo 'ERROR';
    die();
}

header('Content-Type: application/json; charset=utf-8');

$id = $_POST['user_id'];
$serial = $_POST['serial_no'];

try {
    $query = "UPDATE user_accounts SET card_serial_no = ? WHERE user_id = ?";
    $statement = $con->prepare($query);
    $statement->bind_param('si', $serial, $id);
    $statement->execute();
echo 'OK';
}
catch (Exception $ex) {
    http_response_code(500);
    echo 'ERROR';
}
?>