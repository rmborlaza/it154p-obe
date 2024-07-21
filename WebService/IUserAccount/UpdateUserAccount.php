<?php 
include_once('../connects.php');

$paramsSet = isset($_POST['user_id']) && isset($_POST['username']) && isset($_POST['first_name']) && isset($_POST['last_name']) && isset($_POST['account_type']);

if (!$paramsSet)
{
    http_response_code(401);
    echo 'ERROR';
    die();
}

$id = $_POST['user_id'];
$username = $_POST['username'];
$firstName = $_POST['first_name'];
$lastName = $_POST['last_name'];
$accountType = $_POST['account_type'];
$type = 0;

if ($accountType == 'system') {
    $type = 1;
}

$query = "UPDATE user_accounts SET username=?, first_name=?, last_name=?, is_system_account=? WHERE user_id = ?";
$statement = $con->prepare($query);
$statement->bind_param('sssii', $username, $firstName, $lastName, $type, $id);
$statement->execute();

try {
    echo 'OK';
}
catch (Exception $ex) {
    http_response_code(500);
    echo 'ERROR';
}

?>