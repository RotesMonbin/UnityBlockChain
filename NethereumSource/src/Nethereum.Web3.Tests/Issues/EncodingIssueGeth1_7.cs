﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3.Accounts;
using Xunit;
using BigInteger = System.Numerics.BigInteger;

namespace Nethereum.Web3.Tests.Issues
{
    public class EncodingIssueGeth1_7
    {
        /*
         ﻿[{"constant":true,"inputs":[],"name":"InvestmentsCount","outputs":[{"name":"","type":"uint256"}],"payable":false,"type":"function"},{"constant":true,"inputs":[],"name":"GetAllInvestments","outputs":[{"name":"ids","type":"uint256[]"},{"name":"addresses","type":"address[]"},{"name":"chargerIds","type":"uint256[]"},{"name":"balances","type":"uint256[]"},{"name":"states","type":"bool[]"}],"payable":false,"type":"function"},{"constant":false,"inputs":[{"name":"_from","type":"address"},{"name":"_charger","type":"uint256"}],"name":"investInQueue","outputs":[{"name":"success","type":"bool"}],"payable":true,"type":"function"},{"constant":false,"inputs":[{"name":"_newCharge","type":"uint256"}],"name":"addCharge","outputs":[],"payable":false,"type":"function"},{"constant":true,"inputs":[],"name":"getChargers","outputs":[{"name":"chargers","type":"uint256[]"}],"payable":false,"type":"function"}]
        60606040526108f2806100126000396000f3606060405260e060020a6000350463472ad331811461004a5780637996c88714610058578063b4821203146102ff578063dbda4c0814610400578063fa82518514610449575b610002565b34610002576104af60005481565b346100025760408051602080820183526000808352835180830185528181528451808401865282815285518085018752838152865180860188528481528751808701895285815288518088018a5286815289518089018b528781528a51808a018c528881528b51998a018c52888a5288549b516104c19c989a979996989597959694959394929391929087908059106100ee5750595b908082528060200260200182016040528015610105575b509550866040518059106101165750595b90808252806020026020018201604052801561012d575b5094508660405180591061013e5750595b908082528060200260200182016040528015610155575b509350866040518059106101665750595b90808252806020026020018201604052801561017d575b5092508660405180591061018e5750595b9080825280602002602001820160405280156101a5575b509150600090505b60005481101561065757600180548290811015610002579060005260206000209060050201600050548651879083908110156100025760209081029091010152600180548290811015610002579060005260206000209060050201600050600101548551600160a060020a03909116908690839081101561000257600160a060020a03909216602092830290910190910152600180548290811015610002579060005260206000209060050201600050600201600050548482815181101561000257602090810290910101526001805482908110156100025790600052602060002090600502016000506003016000505483828151811015610002576020908102909101015260018054829081101561000257906000526020600020906005020160005060040154825160ff9091169083908390811015610002579115156020928302909101909101526001016101ad565b6105f760043560243560008082151561032f57600280546000908110156100025760009182526020909120015492505b61066a84846040805160a081018252600080825260208201819052918101829052606081018290526080810182905281905b6000548210156106c05784600160a060020a0316600160005083815481101561000257906000526020600020906005020160005060010154600160a060020a03161480156103d1575083600160005083815481101561000257906000526020600020906005020160005060020154145b156107a7576001805483908110156100025790600052602060002090600502016000505492505b505092915050565b346100025761060b600435600280546001810180835582818380158290116106a6576000838152602090206106a69181019083015b808211156106bc5760008155600101610435565b34610002576040805160208082018352600082526002805484518184028101840190955280855261060d94928301828280156104a557602002820191906000526020600020905b81548152600190910190602001808311610490575b5050505050905090565b60408051918252519081900360200190f35b60405180806020018060200180602001806020018060200186810386528b8181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f15090500186810385528a8181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f1509050018681038452898181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f1509050018681038352888181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f1509050018681038252878181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f1509050019a505050505050505050505060405180910390f35b604080519115158252519081900360200190f35b005b60405180806020018281038252838181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f1509050019250505060405180910390f35b50939a9299509097509550909350915050565b905034600160005060018303815481101561000257906000526020600020906005020160005060030180549091019055600191505b5092915050565b5050506000928352506020909120018190555b50565b5090565b6107b2858560a06040519081016040528060008152602001600081526020016000815260200160008152602001600081526020015060a0604051908101604052806000815260200160008152602001600081526020016000815260200160008152602001506107bd8360008181526003602052604090205460ff60a060020a9091041615156106b9576000818152600360205260409020805474ff0000000000000000000000000000000000000000191660a060020a179055600280546001810180835582818380158290116106a6576000838152602090206106a6918101908301610435565b600190910190610361565b8051935090506103f8565b60008054600190810191829055600160a060020a0386166020840152604083018590529082528054808201808355828183801582901161085e5760050281600502836000526020600020918201910161085e91905b808211156106bc57600080825560018201805473ffffffffffffffffffffffffffffffffffffffff1916905560028201819055600382015560048101805460ff19169055600501610812565b50505060009283525060209182902083516005909202019081559082015160018201805473ffffffffffffffffffffffffffffffffffffffff19166c0100000000000000000000000092830292909204919091179055604082015160028201556060820151600382015560808201516004909101805460ff191660f860020a9283029290920491909117905590508061069f56
        0xc74c12278aa650f41c20cc66dc488c583ab780cf
        0x1dC0Cdd495d2Fd22aaf216b82D14936e1fCD8b40 
        */
        [Fact]
        public async void Test()
        {
            var abi =
                "[{'constant':true,'inputs':[],'name':'InvestmentsCount','outputs':[{'name':'','type':'uint256'}],'payable':false,'type':'function'},{'constant':true,'inputs':[],'name':'GetAllInvestments','outputs':[{'name':'ids','type':'uint256[]'},{'name':'addresses','type':'address[]'},{'name':'chargerIds','type':'uint256[]'},{'name':'balances','type':'uint256[]'},{'name':'states','type':'bool[]'}],'payable':false,'type':'function'},{'constant':false,'inputs':[{'name':'_from','type':'address'},{'name':'_charger','type':'uint256'}],'name':'investInQueue','outputs':[{'name':'success','type':'bool'}],'payable':true,'type':'function'},{'constant':false,'inputs':[{'name':'_newCharge','type':'uint256'}],'name':'addCharge','outputs':[],'payable':false,'type':'function'},{'constant':true,'inputs':[],'name':'getChargers','outputs':[{'name':'chargers','type':'uint256[]'}],'payable':false,'type':'function'}]";

            var byteCode = "0x60606040526108f2806100126000396000f3606060405260e060020a6000350463472ad331811461004a5780637996c88714610058578063b4821203146102ff578063dbda4c0814610400578063fa82518514610449575b610002565b34610002576104af60005481565b346100025760408051602080820183526000808352835180830185528181528451808401865282815285518085018752838152865180860188528481528751808701895285815288518088018a5286815289518089018b528781528a51808a018c528881528b51998a018c52888a5288549b516104c19c989a979996989597959694959394929391929087908059106100ee5750595b908082528060200260200182016040528015610105575b509550866040518059106101165750595b90808252806020026020018201604052801561012d575b5094508660405180591061013e5750595b908082528060200260200182016040528015610155575b509350866040518059106101665750595b90808252806020026020018201604052801561017d575b5092508660405180591061018e5750595b9080825280602002602001820160405280156101a5575b509150600090505b60005481101561065757600180548290811015610002579060005260206000209060050201600050548651879083908110156100025760209081029091010152600180548290811015610002579060005260206000209060050201600050600101548551600160a060020a03909116908690839081101561000257600160a060020a03909216602092830290910190910152600180548290811015610002579060005260206000209060050201600050600201600050548482815181101561000257602090810290910101526001805482908110156100025790600052602060002090600502016000506003016000505483828151811015610002576020908102909101015260018054829081101561000257906000526020600020906005020160005060040154825160ff9091169083908390811015610002579115156020928302909101909101526001016101ad565b6105f760043560243560008082151561032f57600280546000908110156100025760009182526020909120015492505b61066a84846040805160a081018252600080825260208201819052918101829052606081018290526080810182905281905b6000548210156106c05784600160a060020a0316600160005083815481101561000257906000526020600020906005020160005060010154600160a060020a03161480156103d1575083600160005083815481101561000257906000526020600020906005020160005060020154145b156107a7576001805483908110156100025790600052602060002090600502016000505492505b505092915050565b346100025761060b600435600280546001810180835582818380158290116106a6576000838152602090206106a69181019083015b808211156106bc5760008155600101610435565b34610002576040805160208082018352600082526002805484518184028101840190955280855261060d94928301828280156104a557602002820191906000526020600020905b81548152600190910190602001808311610490575b5050505050905090565b60408051918252519081900360200190f35b60405180806020018060200180602001806020018060200186810386528b8181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f15090500186810385528a8181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f1509050018681038452898181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f1509050018681038352888181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f1509050018681038252878181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f1509050019a505050505050505050505060405180910390f35b604080519115158252519081900360200190f35b005b60405180806020018281038252838181518152602001915080519060200190602002808383829060006004602084601f0104600302600f01f1509050019250505060405180910390f35b50939a9299509097509550909350915050565b905034600160005060018303815481101561000257906000526020600020906005020160005060030180549091019055600191505b5092915050565b5050506000928352506020909120018190555b50565b5090565b6107b2858560a06040519081016040528060008152602001600081526020016000815260200160008152602001600081526020015060a0604051908101604052806000815260200160008152602001600081526020016000815260200160008152602001506107bd8360008181526003602052604090205460ff60a060020a9091041615156106b9576000818152600360205260409020805474ff0000000000000000000000000000000000000000191660a060020a179055600280546001810180835582818380158290116106a6576000838152602090206106a6918101908301610435565b600190910190610361565b8051935090506103f8565b60008054600190810191829055600160a060020a0386166020840152604083018590529082528054808201808355828183801582901161085e5760050281600502836000526020600020918201910161085e91905b808211156106bc57600080825560018201805473ffffffffffffffffffffffffffffffffffffffff1916905560028201819055600382015560048101805460ff19169055600501610812565b50505060009283525060209182902083516005909202019081559082015160018201805473ffffffffffffffffffffffffffffffffffffffff19166c0100000000000000000000000092830292909204919091179055604082015160028201556060820151600382015560808201516004909101805460ff191660f860020a9283029290920491909117905590508061069f56";

            var web3 = new Web3(ClientFactory.GetClient());

            var gethTester = GethTesterFactory.GetLocal(web3);

            await gethTester.UnlockAccount();
            var receipt = await gethTester.DeployTestContractLocal(byteCode);

            var contract = web3.Eth.GetContract(abi, receipt.ContractAddress);
            var addChargeFunction = contract.GetFunction("addCharge");

            var gas = await addChargeFunction.EstimateGasAsync(gethTester.Account, null,  null, 20);
            var tx = await addChargeFunction.SendTransactionAsync(gethTester.Account, gas, null, 20);
            tx = await addChargeFunction.SendTransactionAsync(gethTester.Account, gas, null, 30);
            receipt = await gethTester.GetTransactionReceipt(tx);


            var chargers = contract.GetFunction("getChargers");

            var result = await chargers.CallAsync<List<BigInteger>>();

            Assert.Equal(20, result[0]);
            Assert.Equal(30, result[1]);
        }
    }
}
