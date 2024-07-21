<?php
include_once('../connects.php');

header('Content-Type: application/json; charset=utf-8');

$paramsSet = isset($_GET['user_id']);

$usersJson = array();

if ($paramsSet) {
    $id = (int)$_GET['user_id'];
    $query = "SELECT user_id, username, first_name, last_name, CONVERT_TZ(registration, '+00:00', '+08:00') as registration, is_system_account FROM user_accounts WHERE user_id=?";

    $statement = $con->prepare($query);
    $statement->bind_param('i', $id);
    $statement->execute();

    $result = $statement->get_result();
    $numRows = mysqli_num_rows($result);

    if ($numRows > 0) {
        $user = $result->fetch_assoc();
        $type = 'user';
        if ($user['is_system_account']) {
            $type = 'system';
        }

        $userJson = array('user_id'=>$user['user_id'], 'first_name'=>$user['first_name'], 'last_name'=>$user['last_name'], 'username'=>$user['username'], 'registration'=>$user['registration'], 'account_type'=>$type);
        echo json_encode($userJson);
    }
}
else {
    $query = "SELECT user_id, username, first_name, last_name, CONVERT_TZ(registration, '+00:00', '+08:00') as registration, is_system_account FROM user_accounts";

    $statement = $con->prepare($query);
    $statement->execute();
    
    $result = $statement->get_result();
    $users = $result->fetch_all();

    foreach ($users as $user) {
        $type = 'user';
        if ($user[5] == 1)
            $type = 'system';

        $userJson = array('user_id'=>$user[0], 'username'=>$user[1], 'first_name'=>$user[2], 'last_name'=>$user[3], 'registration'=>$user[4], 'account_type'=>$type);
        array_push($usersJson, $userJson);
    }
    echo json_encode($usersJson);
}

?>