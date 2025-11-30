<?php

include '../ky_base.php';

$sizeID = req('sizeID');

// Fetch quantity based on sizeID
$stm = $_db->prepare('SELECT quantity FROM sizes WHERE sizeID = ?');
$stm->execute([$sizeID]);
$size = $stm->fetch();

if ($size) {
    echo $size->quantity;
} else {
    echo 0;
}
?>
