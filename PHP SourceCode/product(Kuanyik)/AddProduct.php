<head>        
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script src="../js/app.js" type="text/javascript"></script>
    <script src="../js/script.js"></script>
    <link href="../css/kycss.css" rel="stylesheet" type="text/css"/>
    <link href="../css/app.css" rel="stylesheet" type="text/css"/></head>
<title>Add New Product</title>
<?php
include '../ky_base.php';
include '../_head.php';
$adminID = "";
if (!isset($_SESSION)) {
    session_start();
}
if (isset($_SESSION['user']) && !empty($_SESSION['user'])) {
    $_user = $_SESSION['user'];
    $adminID = $_user->userID;
}

// Fetch categories
$categoryName = $_db->query('SELECT categoryID, categoryName FROM category')
        ->fetchAll(PDO::FETCH_KEY_PAIR);

if (is_post()) {
    $productName = req('productName');
    $productDesc = req('productDesc');
    $price = req('price');
    $f = get_file('productPhoto1');
    $f1 = get_file('productPhoto2');
    $categoryID = req('categoryID');
    $sizes = req('sizes');
    $quantities = req('quantities'); // For handling quantities per size
    // Validate sizes
    // ... [existing validation code for sizes and quantities]
    // Validate: productName
    if ($productName == '') {
        $_err['productName'] = '<span class="err-message">*Required</span>';
    } else if (is_categoryexists($productName, 'product', 'productName')) {
        $_err['productName'] = '<span class="err-message">*Product Exists</span>';
    } else if (strlen($productName) > 100) {
        $_err['productName'] = '<span class="err-message">*Maximum length 100</span>';
    } else if (!preg_match('/^[a-zA-Z\s]+$/', $productName)) {
        $_err['productName'] = '<span class="err-message">*Only alphabetic characters and spaces are allowed</span>';
    }

    // Validate: productDesc
    if ($productDesc == '') {
        $_err['productDesc'] = '<span class="err-message">*Required</span>';
    } else if (strlen($productDesc) > 500) {
        $_err['productDesc'] = '<span class="err-message">*Maximum 500 characters</span>';
    }

    // Validate: photo (file)
    if (!$f || !$f1) {
        $_err['photos'] = '<span class="err-message">*Both images are required</span>';
    } else if (!str_starts_with($f->type, 'image/') || !str_starts_with($f1->type, 'image/')) {
        $_err['photos'] = '<span class="err-message">*Both images must be images</span>';
    } else if ($f->size > 1 * 1024 * 1024 || $f1->size > 1 * 1024 * 1024) {
        $_err['photos'] = '<span class="err-message">*Both images must be less than 1MB</span>';
    }

    // Validate categoryID
    if ($categoryID == '') {
        $_err['categoryID'] = '<span class="err-message">*Please select a category</span>';
    } else if ($categoryID == 'Select a category') {
        $_err['categoryID'] = '<span class="err-message">*Please select a category</span>';
    } else if (!array_key_exists($categoryID, $categoryName)) {
        $_err['categoryID'] = '<span class="err-message">*Invalid category</span>';
    }

    // Validate: price
    if ($price == '') {
        $_err['price'] = '<span class="err-message">*Required</span>';
    } else if (!is_money($price)) {
        $_err['price'] = '<span class="err-message">*Must be money</span>';
    } else if ($price < 0.01 || $price > 99999.99) {
        $_err['price'] = '<span class="err-message">*Must be between 0.01 - 99999.99</span>';
    }
// Validate: sizes
    if ($sizes == '') {
        $_err['sizes'] = '<span class="err-message">*Required</span>';
    } else {
        $sizeArray = explode(',', $sizes);
        $quantityArray = explode(',', $quantities);

        if (count($sizeArray) != count($quantityArray)) {
            $_err['sizes'] = '<span class="err-message">*Sizes and quantities must match</span>';
        } else {
            foreach ($sizeArray as $size) {
                if (trim($size) == '') {
                    $_err['sizes'] = '<span class="err-message">*Invalid size</span>';
                    break;
                }
            }

            foreach ($quantityArray as $quantity) {
                if (!is_numeric($quantity) || $quantity < 0) {
                    $_err['quantities'] = '<span class="err-message">*Invalid quantity</span>';
                    break;
                }
            }
        }
    }

// Validate: quantities
    if ($quantities == '') {
        $_err['quantities'] = '<span class="err-message">*Required</span>';
    }

    // DB operation
    if (!$_err) {
        // Save photos
        $photo = save_photo($f, '../photos');
        $photo2 = save_photo($f1, '../photos');

        // Insert product into the product table
        $stm = $_db->prepare('
            INSERT INTO product (productName, productDesc, price, productPhoto1, productPhoto2, status, categoryID)
            VALUES (?, ?, ?, ?, ?, ?, ?)
        ');
        $stm->execute([$productName, $productDesc, $price, $photo, $photo2, "Available", $categoryID]);

        // Get the last inserted productID
        $productID = $_db->lastInsertId();

        // Insert sizes and corresponding quantities into the sizes table
        $stm = $_db->prepare('INSERT INTO sizes (productID, size, quantity) VALUES (?, ?, ?)');
        foreach ($sizeArray as $index => $size) {
            $quantity = $quantityArray[$index];
            $stm->execute([$productID, $size, $quantity]);
        }

        temp('success', '<span class="success-message">Product uploaded successfully!</span>'); // Set a temporary success message
        //redirect('AddProduct.php');
    }
}

// ----------------------------------------------------------------------------

$_title = 'Product | Insert';
?>

<div class="row clearfix">
    <div class="col-1" style="padding:0 200px;margin-bottom: 20px;text-align: center;">
        <h1>New Product</h1>
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
                <label class="category-label" for="categoryID">Category</label>            <?= err('categoryID') ?>
                <?= html_select('product-input', 'categoryID', $categoryName) ?>
            </div>
            <div class="col-1" style="margin-bottom:20px;">
                <label class="category-label" for="productName">Product Name</label> <?= err('productName') ?>
                <?= html_text('product-input', 'productName', 'Enter Product Name', 'maxlength="100"') ?>
            </div>
            <div class="col-1" style="margin-bottom:20px;">
                <label class="category-label" for="productDesc">Product Description</label><?= err('productDesc') ?>
                <?= html_text('product-input', 'productDesc', 'Product Description', 'maxlength="200"') ?>
            </div>
            <div class="col-1" style="margin-bottom:20px;">
                <label class="category-label" for="price">Price</label><?= err('price') ?>
                <?= html_number('product-input', 'price', 'step="0.01" min="0.01" max="99999.99"') ?>
            </div>
            <div class="col-1" style="margin-bottom:20px;">
                <label class="category-label" for="sizes">Sizes</label><?= err('sizes') ?>
                <?= html_text('product-input', 'sizes', 'Enter sizes separated by commas (e.g. S, M, L)', '') ?>
            </div>
            <div class="col-1" style="margin-bottom:20px;">
                <label class="category-label" for="quantities">Quantities</label><?= err('quantities') ?>
                <?= html_text('product-input', 'quantities', 'Enter quantities corresponding to sizes (e.g. 10, 15, 20)', '') ?>
            </div>
            <div class="col-1" style="margin-bottom:20px;">
                <label class="category-label" for="productPhoto1">Photo</label>
                <?= err('photos') ?>
                <br>
                <div style="display: flex;">
                    <label class="product-upload" tabindex="0" style="margin-right: 20px;">
                        <?= html_file('productPhoto1', 'image/*', 'hidden') ?>
                        <img src="../images/photo.jpg">
                    </label>

                    <label class="product-upload" tabindex="0">
                        <?= html_file('productPhoto2', 'image/*', 'hidden') ?>
                        <img src="../images/photo.jpg">
                    </label>
                    <?= err('productPhoto2') ?>
                </div>
            </div>
            <section style="text-align: center; margin: 40px 0;">
                <button type="reset" class="reverse-button">Reset</button>
                <button class="action-button">Submit</button>
            </section>
        </form>

        <?php
        include '../admin/footer.php';
        ?>
