<?php
include '../ky_base.php';

// ----------------------------------------------------------------------------

if (is_get()) {
    $productID = req('productID');

    // Fetch product details
    $stm = $_db->prepare('SELECT * FROM product WHERE productID = ?');
    $stm->execute([$productID]);
    $p = $stm->fetch();

    // Fetch sizes for the product
    $stm_sizes = $_db->prepare('SELECT * FROM sizes WHERE productID = ?');
    $stm_sizes->execute([$productID]);
    $sizes = $stm_sizes->fetchAll();

    if (!$p) {
        redirect('ProductList.php');
    }

    extract((array) $p);
    $_SESSION['productPhoto1'] = $p->productPhoto1;
    $_SESSION['productPhoto2'] = $p->productPhoto2;
}

if (is_post()) {
    $productID = req('productID');

    // Fetch product details again for POST request
    $stm = $_db->prepare('SELECT * FROM product WHERE productID = ?');
    $stm->execute([$productID]);
    $p = $stm->fetch();

    if (!$p) {
        redirect('ProductList.php');
    }

    $productName = req('productName');
    $productDesc = req('productDesc');
    $price = req('price');
    $status = req('status'); // Get status
    $sizeID = req('size'); // Get selected sizeID
    $quantity = req('quantity'); // Get updated quantity for the size

    $f = get_file('productPhoto1');
    $f2 = get_file('productPhoto2');
    $photo = $_SESSION['productPhoto1'];
    $photo2 = $_SESSION['productPhoto2'];

    // Validate: product name
    if ($productName == '') {
        $_err['productName'] = '<span class="err-message">*Required</span>';
    } else {
        // Check if the product name already exists in another product, excluding the current one
        $stm_check = $_db->prepare('SELECT COUNT(*) FROM product WHERE productName = ? AND productID != ?');
        $stm_check->execute([$productName, $productID]);
        $exists = $stm_check->fetchColumn();

        if ($exists) {
            $_err['productName'] = '<span class="err-message">*Product Exists</span>';
        } else if (strlen($productName) > 100) {
            $_err['productName'] = '<span class="err-message">*Maximum 100 characters</span>';
        }
    }


    // Validate: productDesc
    if ($productDesc == '') {
        $_err['productDesc'] = '<span class="err-message">*Required</span>';
    } else if (strlen($productDesc) > 500) {
        $_err['productDesc'] = '<span class="err-message">*Maximum 500 characters</span>';
    }


    // Validate: price
    if ($price == '') {
        $_err['price'] = '<span class="err-message">*Required</span>';
    } else if (!is_money($price)) {
        $_err['price'] = '<span class="err-message">*Must be money</span>';
    } else if ($price < 0.01 || $price > 99999.99) {
        $_err['price'] = '<span class="err-message">*Must be between 0.01 - 99999.99</span>';
    }

    // Validate: photo (file)
    if ($f) {
        if (!str_starts_with($f->type, 'image/')) {
            $_err['productPhoto1'] = '<span class="err-message">*Must be an image</span>';
        } else if ($f->size > 1 * 1024 * 1024) {
            $_err['productPhoto1'] = '<span class="err-message">*Maximum 1MB</span>';
        } else {
            // Update the image name and path
            $photo = save_photo($f, '../photos');
            $_SESSION['productPhoto1'] = $photo;
        }
    }
    if ($f2) {
        if (!str_starts_with($f2->type, 'image/')) {
            $_err['productPhoto2'] = '<span class="err-message">*Must be an image</span>';
        } else if ($f2->size > 1 * 1024 * 1024) {
            $_err['productPhoto2'] = '<span class="err-message">*Maximum 1MB</span>';
        } else {
            // Update the image name and path
            $photo2 = save_photo($f2, '../photos');
            $_SESSION['productPhoto2'] = $photo2;
        }
    }

    // Validate: size
    if ($sizeID == '') {
        $_err['size'] = '<span class="err-message">Required</span>';
        // Fetch the size list again if size validation passed
        $stm_sizes = $_db->prepare('SELECT * FROM sizes WHERE productID = ?');
        $stm_sizes->execute([$productID]);
        $sizes = $stm_sizes->fetchAll();
    } else {
        // Fetch the size list again if size validation passed
        $stm_sizes = $_db->prepare('SELECT * FROM sizes WHERE productID = ?');
        $stm_sizes->execute([$productID]);
        $sizes = $stm_sizes->fetchAll();
    }


    // Validate: quantity
    if ($quantity === '' || $quantity === null) {
        $_err['quantity'] = '<span class="err-message">*Required</span>'; // Check for null or empty value
    } else if ($sizeID === '' || $sizeID === null) {
        $_err['quantity'] = '<span class="err-message">*Please Select Size</span>'; // Ensure a size is selected before checking quantity
    } else if (!is_numeric($quantity) || $quantity < 0 || $quantity > 100) {
        $_err['quantity'] = '<span class="err-message">*Invalid quantity</span>'; // Check if quantity is numeric and within valid range
    }


    // Validate: status
    if ($status == '') {
        $_err['status'] = '<span class="err-message">*Required</span>';
    } else if (!in_array($status, ['Available', 'Disable'])) {
        $_err['status'] = '<span class="err-message">*Invalid status</span>';
    }

    // DB operation
    if (!$_err) {
        // Transaction to ensure both product and size are updated together
        $_db->beginTransaction();

        try {
            // Update product details
            if ($f) {
                unlink("../photos/$photo");
                $photo = save_photo($f, '../photos');
            }
            if ($f2) {
                unlink("../photos/$photo2");
                $photo2 = save_photo($f2, '../photos');
            }

            $stm = $_db->prepare('
                UPDATE product
                SET productName = ?, productDesc = ?, price = ?, productPhoto1 = ?, productPhoto2 = ?, status = ?
                WHERE productID = ?
            ');
            $stm->execute([$productName, $productDesc, $price, $photo, $photo2, $status, $productID]);

            // Update size and quantity in sizes table
            if ($sizeID && $quantity !== null) {
                $stm_size = $_db->prepare('
                    UPDATE sizes
                    SET quantity = ?
                    WHERE sizeID = ? AND productID = ?
                ');
                $stm_size->execute([$quantity, $sizeID, $productID]);
            }

            // Commit the transaction
            $_db->commit();

            // Redirect and display success message
            temp('success', '<span class="success-message">Product updated!</span>'); // Set a temporary success message
        } catch (Exception $e) {
            // Rollback the transaction in case of error
            $_db->rollBack();
            temp('error', 'Failed to update record: ' . $e->getMessage());
        }
    }
}

// ----------------------------------------------------------------------------

$_title = 'Product | Update';
include '../_head.php';
?>
<head>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script src="../js/app.js" type="text/javascript"></script>
    <script src="../js/script.js"></script>
    <link href="../css/kycss.css" rel="stylesheet" type="text/css"/>
    <link href="../css/app.css" rel="stylesheet" type="text/css"/>
    <title>Update Product</title>
</head>
<body>
    <!-- HTML form as before -->
    <div class="row clearfix">
        <div class="col-1" style="padding:0 200px;margin-bottom: 20px;text-align: center;">
            <h1>Update Category</h1>
            <?= temp('success') ?>
        </div>
        <div class="col-1" style="padding: 0 200px;">
            <button data-get="ProductList.php" class="reverse-button">Back</button>
        </div>
    </div>

    <div class="row clearfix nopadding-top">
        <div class="col-1">
            <form method="post" class="product-form" enctype="multipart/form-data" novalidate>
                <div class="col-1" style="margin-bottom:20px;">

                    <label class="category-label" for="productName">Product Name</label><?= err('productName') ?>
                    <?= html_text('product-input', 'productName', 'Enter Product Name', 'maxlength="100"') ?>
                </div>
                <div class="col-1" style="margin-bottom:20px;">

                    <label class="category-label" for="productDesc">Product Description</label><?= err('productDesc') ?>
                    <?= html_text('product-input', 'productDesc', 'Enter Product Description', 'maxlength="500"') ?>
                </div>
                <div class="col-1" style="margin-bottom:20px;">

                    <label class="category-label" for="price">Price</label><?= err('price') ?>
                    <?= html_number('product-input', 'price', 'Enter Price', 'maxlength="100"') ?>
                </div>
                <div class="col-1" style="margin-bottom:20px;">

                    <label class="category-label">Select 2 Images</label><?= err('productPhoto1') ?>
                    <br>
                    <div style="display: flex;">
                        <label class="product-upload" tabindex="0" style="margin-right: 20px;">
                            <?= html_file('productPhoto1', 'image/*', 'hidden') ?>
                            <img src="../photos/<?= $_SESSION['productPhoto1'] ?>" alt="Photo 1">
                        </label>

                        <label class="product-upload" tabindex="0">
                            <?= html_file('productPhoto2', 'image/*', 'hidden') ?>
                            <img src="../photos/<?= $_SESSION['productPhoto2'] ?>" alt="Photo 2">
                        </label>
                    </div>
                </div>
                <div class="col-1" style="margin-bottom:20px;">

                    <label class="category-label" for="size">Size</label> <?= err('size') ?>
                    <select id="size" name="size" class="product-input">
                        <option value="">Select Size</option>
                        <?php foreach ($sizes as $size): ?>
                            <option value="<?= $size->sizeID ?>"><?= $size->size ?></option>
                        <?php endforeach; ?>
                    </select>
                </div>
                <div class="col-1" style="margin-bottom:20px;">

                    <label class="category-label" for="quantity">Quantity</label><?= err('quantity') ?>
                    <input type="number" id="quantity" name="quantity" class="product-input" placeholder="Enter Quantity">
                </div>
                <div class="col-1" style="margin-bottom:20px;">

                    <label class="category-label" for="status">Status</label> <?= err('status') ?>
                    <select id="status" name="status" class="product-input">
                        <option value="">Select Status</option>
                        <option value="Available" <?= $status == 'Available' ? 'selected' : '' ?>>Available</option>
                        <option value="Disable" <?= $status == 'Disable' ? 'selected' : '' ?>>Disable</option>
                    </select>
                </div>

                <section style="text-align: center; margin: 40px 0;">
                    <button type="reset" class="reverse-button">Reset</button>
                    <button class="action-button">Submit</button>
                </section>
            </form>
        </div>
    </div>

    <script>
        $(document).ready(function () {
            // When size is changed
            $('#size').change(function () {
                var sizeID = $(this).val();

                // If no size selected, clear the quantity field
                if (!sizeID) {
                    $('#quantity').val('');
                } else {
                    $.ajax({
                        url: 'getQuantity.php',
                        type: 'POST',
                        data: {sizeID: sizeID},
                        success: function (response) {
                            $('#quantity').val(response);
                        }
                    });
                }
            });
        });
    </script>
</body>
<?php include '../admin/footer.php'; ?>
