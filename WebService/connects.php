<?php

$db_host = 'localhost';
$db_user = 'root';
$db_pass = '';
$db_name = 'it154p';

$con = mysqli_connect($db_host,$db_user,$db_pass,$db_name);

if(!$con)
{
    die("Unable to connect to database");
}

?>