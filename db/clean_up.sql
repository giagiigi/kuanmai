set SQL_SAFE_UPDATES=0;

delete from Buy_Price;
delete from Buy_Price_Detail;
delete from Buy_Order;
delete from Buy_Order_Detail;
delete from Buy;
delete from Buy_Detail;
delete from Back_Sale;
delete from Back_Sale_Detail;
delete from Back_Stock;
delete from Back_Stock_Detail;
delete from Leave_Stock;
delete from Leave_Stock_Detail;
delete from Enter_Stock;
delete from Enter_Stock_Detail;
delete from Sale;
delete from Sale_Detail;
delete from Mall_Product;
delete from Mall_Product_Sku;
delete from Product;
delete from Sale_SyncTime;
delete from SyncWithMall;
delete from Product;
delete from Stock_Pile;
delete from Stock_Waste;
update Shop set Parent_Shop_ID=0 where Parent_Shop_ID>0;
delete from Shop_Child_Request;
delete from Product_Supplier;
