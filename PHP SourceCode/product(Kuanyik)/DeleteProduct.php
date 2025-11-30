<?php

include '../ky_base.php';

// ----------------------------------------------------------------------------

if (is_post()) {
    $productID = req('productID');

    // Delete sizes related to the product
    $stm_sizes = $_db->prepare('SELECT sizeID FROM sizes WHERE productID = ?');
    $stm_sizes->execute([$productID]);
    $sizes = $stm_sizes->fetchAll(PDO::FETCH_COLUMN);

    foreach ($sizes as $sizeID) {
        // Delete each size entry
        $stm_delete_size = $_db->prepare('DELETE FROM sizes WHERE sizeID = ?');
        $stm_delete_size->execute([$sizeID]);
    }

    // Delete product photos
    $stm_product = $_db->prepare('SELECT productPhoto1, productPhoto2 FROM product WHERE productID = ?');
    $stm_product->execute([$productID]);
    $photos = $stm_product->fetch(PDO::FETCH_ASSOC);
    if ($photos) {
        unlink("../photos/" . $photos['productPhoto1']);
        unlink("../photos/" . $photos['productPhoto2']);
    }

    // Delete the product record
    $stm_product_delete = $_db->prepare('DELETE FROM product WHERE productID = ?');
    $stm_product_delete->execute([$productID]);

    temp('info', 'Product and related sizes deleted');
}

// Redirect back to product list
redirect('ProductList.php');

// ----------------------------------------------------------------------------
