using MilkStore.Core.Base;
using MilkStore.Core.Constants;

public class TestService
{
    public Task<int> Test(int a)
    {
        if (a == 0)
        {
            throw new BaseException.ErrorException(StatusCodes.BadRequest, ErrorCode.BadRequest, "Fix lỗi ở đây");
        }
        return Task.FromResult(0);
    }
}