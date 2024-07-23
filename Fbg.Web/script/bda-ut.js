BDA.UT = {
    AssertTrue: function (condition, msg) {
        if (!condition) {
            throw new Error(msg);
        }
    }

}

BDA.SessionDBUT = {
    tbl : 'ut',
    asynch_unittests: function () {

        // delete table UT just in case it exist. 
        BDA.SessionDB.Delete(BDA.SessionDBUT.tbl);
        // now select from table UT, it shoudl be empty
        BDA.SessionDB.SelectAll(BDA.SessionDBUT.tbl).done(function (r) { BDA.UT.AssertTrue(r.length == 0, "delete did not work") });
        // now insert one row into, ensure that one row now exist
        BDA.SessionDB.Insert(BDA.SessionDBUT.tbl, [{ id: 1, name: 'greg', isview: false }], function (r) {
            BDA.SessionDB.SelectAll(BDA.SessionDBUT.tbl).done(function (r) {
                BDA.UT.AssertTrue(r.length == 1, "insert did not work, not one row");
                BDA.UT.AssertTrue(r[0].isview == false, "insert did not work, wrong col value");

                // now insert 2 more rows, 
                BDA.SessionDB.Insert(BDA.SessionDBUT.tbl, [{ id: 2, name: 'greg2', isview: true }]);
                BDA.SessionDB.Insert(BDA.SessionDBUT.tbl, [{ id: 3, name: 'greg3', isview: false }], function (r) {
                    BDA.SessionDB.SelectAll(BDA.SessionDBUT.tbl).done(function (r) {
                        BDA.UT.AssertTrue(r.length == 3, "insert did not work, not 3 row");

                        // now udpate one row, use find to check if it is correct
                        BDA.SessionDB.Update(BDA.SessionDBUT.tbl, { id: 'id', fields: [{ id: 1, name: 'greg', isview: true }] }, function (r) {
                            BDA.SessionDB.SelectAll(BDA.SessionDBUT.tbl).done(function (r) {
                                BDA.UT.AssertTrue(r.length == 3, "not 3 rows");
                            });

                            // test find a bit
                            BDA.SessionDB.Find(BDA.SessionDBUT.tbl, 'id', [1, 2, 3]).done(function (r) {
                                BDA.UT.AssertTrue(r.length == 3, "find did not work");
                                BDA.UT.AssertTrue(r[0].isview == true, "update - row to update did not update");
                                BDA.UT.AssertTrue(r[1].isview == true, "update - other rows were updated mistakenly");
                                BDA.UT.AssertTrue(r[2].isview == false, "update - other rows were updated mistakenly");
                            });

                            // wrong find
                            BDA.SessionDB.Find(BDA.SessionDBUT.tbl, 'id', [1, 2, 333333]).done(function (r) {
                                BDA.UT.AssertTrue(r.length == 2, "find did not work - expect 2 rows");

                                // DO MORE TESTS 
                                BDA.SessionDBUT.asynch_unittests_delete();
                            });

                        });

                    });
                });

            });
        });

    }

    , asynch_unittests_delete: function () {
        // test deleterows - first add some more rows 
        BDA.SessionDB.Insert(BDA.SessionDBUT.tbl, [{ id: 4, name: 'greg4', isview: false }]);
        BDA.SessionDB.Insert(BDA.SessionDBUT.tbl, [{ id: 5, name: 'greg5', isview: false}], function (r) {                                
            BDA.SessionDB.DeleteRows(BDA.SessionDBUT.tbl, { id: 'id', list: [5] }).done(function (r) {

                // are there now 4 rows?
                BDA.SessionDB.SelectAll(BDA.SessionDBUT.tbl).done(function (r) { BDA.UT.AssertTrue(r.length == 4, "delete- expecting 4 rows") });

                // try find removed row 
                BDA.SessionDB.Find(BDA.SessionDBUT.tbl, 'id', [5]).done(function (r) {
                    BDA.UT.AssertTrue(r.length == 0, "delete did not work - expect not find item 5 ");
                });

                // try find not-removed row
                BDA.SessionDB.Find(BDA.SessionDBUT.tbl, 'id', [1,2,3,4]).done(function (r) {
                    BDA.UT.AssertTrue(r.length == 4, "delete did not work- expect not find other items ");
                });

                //DO MORE TESTS
                BDA.SessionDBUT.asynch_unittests_findByFilter();
            });
        });
    }

     , asynch_unittests_findByFilter: function () {

         BDA.SessionDB.FindByFilter(BDA.SessionDBUT.tbl, 'name', 'eg').done(function (r) {
             BDA.UT.AssertTrue(r.length == 4, "FindByFilter - expect 4 rows");
         });

         BDA.SessionDB.FindByFilter(BDA.SessionDBUT.tbl, 'name', 'eg3').done(function (r) {
             BDA.UT.AssertTrue(r.length == 1, "FindByFilter - expect 1 rows");
         });

     }


}

$(function () {
    BDA.Console.log("***************START INITIATING UNIT TESTS ***************************************************");
    BDA.SessionDBUT.asynch_unittests();
    BDA.Console.log("***************END   INITIATING UNIT TESTS (asynch test may still be running *****************");
});