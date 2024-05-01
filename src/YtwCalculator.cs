using System;
using System.Threading.Tasks;
using IMTC.CodeTest.Models;
using IMTC.CodeTest.Core.Services;
using IMTC.CodeTest.Indices.Services;

namespace Imtc.CodeTest.Calculators;

public class YtwCalculator : IYtwCalculator
{

    /*
    The purpose of the YtwCalculator class it is calculates the yield-to-worst for a given bond using external services and index data
    and allow return nullable decimal values .
    */
    /* 
    The class YtwCalculator is declared within namespace Imtc.CodeTest.Calculators and the class implements the interface IYtwCalculator 
    */

    /*
    readonly fields are declare _calculator, _indexProvider. _timeService and store instances of services provided through dependency injection
    */
    private readonly IImtcCalculator _calculator;
    private readonly IIndexProvider _indexProvider;
    private readonly ITimeService _timeService;

    
    /*
    The class constructor takes three parameters calculator , indexProvider and timeService, this parameters are injected via dependency 
    injection allowing the class to use external services
    */
    public YtwCalculator(IImtcCalculator calculator, IIndexProvider indexProvider, ITimeService timeService)
    {
        _calculator = calculator;
        _indexProvider = indexProvider;
        _timeService = timeService;
    }

   /*
   The method calculate the yield to worst (YTW) for a given Bond object , using the current date as the settlement date.
   settlementDate:  refers to the finalization date when a bond trade becomes legally binding, for most municipal bonds,
    the settlement date is two business days after the execution date (T+2).
   The method calls the overloaded method with the settlement date. 
   */
    public async Task<decimal?> CalculateYtwForBond(Bond bond)
    {
        var settlementDate = _timeService.UtcNow.Date;
        return await CalculateYtwForBond(bond, settlementDate);
    }


    /*
   The method calculate the yield to worst (YTW) for a given Bond object on a specific settlement date, determines the appropriate index 
   code based on bond properties and retrives the index data using _indexProvider , calculates YTW using _calculator. 
   */
    public async Task<decimal?> CalculateYtwForBond(Bond bond, DateTime settlementDate)
    {
        //If the input bond is null, an ArgumentNullException is thrown.
        if (bond is null) 
        {
            throw new ArgumentNullException(nameof(bond));
        }

        /*
        The switch expression checks bond properties (CouponType and BondType) to determine the appropriate index code
        default an set to USTR_CMT 
        When a bond has the property CouponType equal to CouponType.Variable, set indexCode to IndexNames.USTR_CMT (U.S. Treasury Constant Maturity bonds).
        When a bond has the property BondType equal to BondType.Municipal, the indexCode is set as IndexNames.MuniAAA (Municipal Bond AAA).
        */
       
        var indexCode = bond switch
        {
            Bond b when b.CouponType == CouponType.Variable => IndexNames.USTR_CMT,
            Bond b when b.BondType == BondType.Municipal => IndexNames.MuniAAA,
            _ => IndexNames.USTR_CMT
        };


        /*
        The method are asynchronous so will await the result of _indexProvider.GetIndex()

        Suggestion : wrap the provider _indexProvider.GetIndex() with try-catch blocks and use a specific exception relevant to operation
        use logging to record the exception and details such exception message on log, for better tracking and analysis .
        */
        var index = await _indexProvider.GetIndex(indexCode, settlementDate);

        
        var ytw = _calculator.CalculateYtw(bond, settlementDate, index);
        return ytw;
    }


}