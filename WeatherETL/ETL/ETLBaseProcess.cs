using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherETL.ETL
{
    public abstract class ETLBaseProcess<TInput, TOutput>
    {
        protected abstract Task<IEnumerable<TInput>> ExtractAsync();

        protected abstract Task LoadAsync(IEnumerable<TOutput> data);

        protected abstract IEnumerable<TOutput> Transform(IEnumerable<TInput> input);

        protected virtual void OnFailed(Exception ex) { }

        protected virtual void OnSucceeded() { }

        public async Task Process() 
        {
            try
            {
                var input = await ExtractAsync();
                var output = Transform(input);
                await LoadAsync(output);
                OnSucceeded();
            }
            catch (Exception ex) 
            {
                OnFailed(ex);
            }
        }
    }
}
