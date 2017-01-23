using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V7.RecyclerView;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using AllContent_Client;
using Square.Picasso;

namespace AndroidContent
{
    public class ItemViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
    {
        private Context context;

        public string url;
        public ImageView Image { get; private set; }
        public TextView Header { get; private set; }
        public TextView Description { get; private set; }
        public TextView Date { get; private set; }

        public ItemViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            // Locate and cache view references:
            context = itemView.Context;
            Image = itemView.FindViewById<ImageView>(Resource.Id.content_imgImageView);
            Header = itemView.FindViewById<TextView>(Resource.Id.content_HeaderTextView);
            Description = itemView.FindViewById<TextView>(Resource.Id.content_DescriptionTextView);
            Date = itemView.FindViewById<TextView>(Resource.Id.content_DateTextView);
            itemView.Clickable = true;
            itemView.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            
            Intent intent = new Intent(context, Java.Lang.Class.FromType(typeof(WebActivity)));
            intent.SetData(Android.Net.Uri.Parse(url));
            context.StartActivity(intent);
        }
    }

    public class ItemScrollListener : RecyclerView.OnScrollListener
    {
        public delegate void LoadMoreEventHandler(object sender, EventArgs e);
        public event LoadMoreEventHandler LoadMoreEvent;

        int visibleItemCount;
        int totalItemCount;
        int firstVisibleItems;
        LinearLayoutManager layoutManager;

        public ItemScrollListener (LinearLayoutManager _mLayoutManager)
        {
            layoutManager = _mLayoutManager;
            
        }
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);

            visibleItemCount = recyclerView.ChildCount;
            totalItemCount = recyclerView.GetAdapter().ItemCount;
            firstVisibleItems = layoutManager.FindFirstVisibleItemPosition();

            if ((visibleItemCount + firstVisibleItems) >= totalItemCount)
            {
                LoadMoreEvent(this, null);
            }
        }
    }

    public class ItemAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private List<ContentUnit> list_cu; 
        private Context context;
        public ItemAdapter(List<ContentUnit> _list_cu, Context _context)
        {
            list_cu = _list_cu;
            context = _context;
        }

        // Create a new CardView (invoked by the layout manager): 
        public override RecyclerView.ViewHolder
            OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the CardView for the item:
            
            View itemView = LayoutInflater.From(parent.Context).
                        Inflate(Resource.Layout.item, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            ItemViewHolder vh = new ItemViewHolder(itemView, OnClick);
            return vh;
        }
        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ItemViewHolder vh = holder as ItemViewHolder;
            vh.url = list_cu[position].URL;
            if (list_cu[position].imgUrl == "")
            {
                vh.Image.Visibility = ViewStates.Gone;
            }
            else {
                Picasso.With(context).Load(list_cu[position].imgUrl).Resize(320, 240).Into(vh.Image);
                vh.Image.Visibility = ViewStates.Visible;

            }
            vh.Header.Text = list_cu[position].header;
            vh.Description.Text = list_cu[position].description;
            vh.Date.Text = list_cu[position].date;
        }

        public override int ItemCount
        {
            get { return list_cu.Count; }
        }

        void OnClick (int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }

}