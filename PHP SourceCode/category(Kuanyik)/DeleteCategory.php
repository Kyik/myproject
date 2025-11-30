<?php

include '../ky_base.php';

// ----------------------------------------------------------------------------

if (is_post()) {
    $categoryID = req('categoryID');

    // Fetch product IDs associated with the category
    $stm = $_db->prepare('SELECT productID FROM product WHERE categoryID = ?');
    $stm->execute([$categoryID]);
    $products = $stm->fetchAll(PDO::FETCH_COLUMN, 0);

    if (!empty($products)) {
        // Delete from sizes where productID matches the product from this category
        $inQuery = implode(',', array_fill(0, count($products), '?'));
        $stm = $_db->prepare("DELETE FROM sizes WHERE productID IN ($inQuery)");
        $stm->execute($products);

        // Now delete the products that belong to this category
        $stm = $_db->prepare('DELETE FROM product WHERE categoryID = ?');
        $stm->execute([$categoryID]);
    }

    // Fetch the photo file names before deleting the category
    $stm = $_db->prepare('SELECT photo, photo2 FROM category WHERE categoryID = ?');
    $stm->execute([$categoryID]);
    $category = $stm->fetch(PDO::FETCH_ASSOC);

    if ($category) {
        // Delete the photos if they exist
        if ($category['photo']) {
            unlink("../photos/" . $category['photo']);
        }
        if ($category['photo2']) {
            unlink("../photos/" . $category['photo2']);
        }

        // Now delete the category
        $stm = $_db->prepare('DELETE FROM category WHERE categoryID = ?');
        $stm->execute([$categoryID]);

        temp('info', 'Category, associated products, and sizes deleted successfully');
    } else {
        temp('error', 'Category not found');
    }
}

// Redirect back to the EditCategory page
redirect('EditCategory.php');

// ----------------------------------------------------------------------------
